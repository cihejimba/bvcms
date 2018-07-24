using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using CmsData;
using Dapper;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Controllers
{
    public class ScriptController : CmsStaffController
    {

#if DEBUG
        [HttpGet, Route("~/Test/{id?}")]
        public ActionResult Test(int? id)
        {
            EmailReplacements.ReCodes();
            return Content("no test");
        }
        [HttpGet, Route("~/Warmup")]
        public ActionResult Warmup()
        {
            return View();
        }
#endif

        public ActionResult RecordTest(int id, string v)
        {
            var o = DbUtil.Db.LoadOrganizationById(id);
            o.AddEditExtra(DbUtil.Db, "tested", v);
            DbUtil.Db.SubmitChanges();
            return Content(v);
        }


#if DEBUG
        [HttpGet, Route("~/TestScript")]
        [Authorize(Roles = "Developer")]
        public ActionResult TestScript()
        {
            //var id = DbUtil.Db.ScratchPadQuery(@"MemberStatusId = 10[Member] AND LastName = 'C*'");

            var file = Server.MapPath("~/test.py");
            var logFile = $"RunPythonScriptInBackground.{DateTime.Now:yyyyMMddHHmmss}";
            string host = Util.Host;

#if false
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                var pe = new PythonModel(host);
                //pe.DictionaryAdd("OrgId", "89658");
                pe.DictionaryAdd("LogFile", logFile);
                PythonModel.ExecutePythonFile(file, pe);
            });
            return View("RunPythonScriptProgress");
#else
            var pe = new PythonModel(host);
            pe.DictionaryAdd("LogFile", logFile);
            ViewBag.Text = PythonModel.ExecutePython(file, pe, fromFile: true);
            return View("Test");
#endif
        }
        [HttpPost, Route("~/TestScript")]
        [ValidateInput(false)]
        [Authorize(Roles = "Developer")]
        public ActionResult TestScript(string script)
        {
            return Content(PythonModel.RunScript(Util.Host, script));
        }
#endif


        [HttpGet, Route("~/RunScript/{name}/{parameter?}/{title?}")]
        public ActionResult RunScript(string name, string parameter = null, string title = null)
        {
            var content = DbUtil.Db.ContentOfTypeSql(name);
            if (content == null)
                return Content("no content");
            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
                p.Add("@" + kv.Key, kv.Value);
            string script = ScriptModel.RunScriptSql(parameter, content, p, ViewBag);

            if (script.StartsWith("Not Authorized"))
                return Message(script);
            ViewBag.Report = name;
            ViewBag.Name = title ?? $"{name.SpaceCamelCase()} {parameter}";
            if (script.Contains("pagebreak"))
            {
                ViewBag.report = PythonModel.PageBreakTables(DbUtil.Db, script, p);
                return View("RunScriptPageBreaks");
            }
            ViewBag.Url = Request.Url?.PathAndQuery;
            var rd = cn.ExecuteReader(script, p, commandTimeout: 1200);
            ViewBag.ExcelUrl = Request.Url?.AbsoluteUri.Replace("RunScript/", "RunScriptExcel/");
            return View(rd);
        }

        [HttpGet, Route("~/RunScriptExcel/{scriptname}/{parameter?}")]
        public ActionResult RunScriptExcel(string scriptname, string parameter = null)
        {
            var content = DbUtil.Db.ContentOfTypeSql(scriptname);
            if (content == null)
                return Message("no content");
            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
                p.Add("@" + kv.Key, kv.Value);
            string script = ScriptModel.RunScriptSql(parameter, content, p, ViewBag);
            if (script.StartsWith("Not Authorized"))
                return Message(script);
            return cn.ExecuteReader(script, p, commandTimeout: 1200).ToExcel("RunScript.xlsx", fromSql: true);
        }

        [HttpGet, Route("~/PyScript/{name}")]
        public ActionResult PyScript(string name, string p1, string p2, string v1, string v2)
        {
            try
            {
                var script = DbUtil.Db.ContentOfTypePythonScript(name);
                if (!script.HasValue())
                    return Message("no script named " + name);

                if (!ScriptModel.CanRunScript(script))
                    return Message("Not Authorized to run this script");
                if (Regex.IsMatch(script, @"model\.Form\b"))
                    return Redirect("/PyScriptForm/" + name);
                script = script.Replace("@P1", p1 ?? "NULL")
                    .Replace("@P2", p2 ?? "NULL")
                    .Replace("V1", v1 ?? "None")
                    .Replace("V2", v2 ?? "None");
                if (script.Contains("@qtagid"))
                {
                    var id = DbUtil.Db.FetchLastQuery().Id;
                    var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                    script = script.Replace("@qtagid", tag.Id.ToString());
                }

                ViewBag.report = name;
                ViewBag.url = Request.Url?.PathAndQuery;
                if (script.Contains("Background Process Completed"))
                {
                    var logFile = $"RunPythonScriptInBackground.{DateTime.Now:yyyyMMddHHmmss}";
                    ViewBag.LogFile = logFile;
                    var qs = Request.Url?.Query;
                    var host = Util.Host;
                    HostingEnvironment.QueueBackgroundWorkItem(ct =>
                    {
                        var qsa = HttpUtility.ParseQueryString(qs ?? "");
                        var pm = new PythonModel(host);
                        pm.DictionaryAdd("LogFile", logFile);
                        foreach (string key in qsa)
                            pm.DictionaryAdd(key, qsa[key]);
                        pm.RunScript(script);
                    });
                    return View("RunPythonScriptProgress");
                }
                var pe = new PythonModel(Util.Host);
                if (script.Contains("@BlueToolbarTagId"))
                {
                    var id = DbUtil.Db.FetchLastQuery().Id;
                    pe.DictionaryAdd("BlueToolbarGuid", id.ToCode());
                }

                foreach (var key in Request.QueryString.AllKeys)
                    pe.DictionaryAdd(key, Request.QueryString[key]);

                pe.RunScript(script);
                if (pe.Output.StartsWith("REDIRECT="))
                {
                    var a = pe.Output.SplitStr("=", 2);
                    return Redirect(a[1].TrimEnd());
                }

                return View(pe);
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }
        [HttpPost, Route("~/RunPythonScriptProgress2")]
        public ActionResult RunPythonScriptProgress2(string logfile)
        {
            var txt = DbUtil.Db.ContentOfTypeText(logfile);
            return Content(txt);
        }
        [Route("~/PyScriptForm/{name}")]
        public ActionResult PyScriptForm(string name)
        {
            return Request.HttpMethod.ToUpper() == "GET"
                ? PyScriptFormGet(name)
                : PyScriptFormPost(name);
        }

        private ActionResult PyScriptFormGet(string name)
        {
            try
            {
                var pe = new PythonModel(Util.Host);
                foreach (var key in Request.QueryString.AllKeys)
                    pe.DictionaryAdd(key, Request.QueryString[key]);
                pe.Data.pyscript = name;
                pe.HttpMethod = "get";
                ScriptModel.Run(name, pe);
                return View(pe);
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }

        private ActionResult PyScriptFormPost(string name)
        {
            try
            {
                var pe = new PythonModel(Util.Host);
                ScriptModel.GetFilesContent(pe);
                foreach (var key in Request.Form.AllKeys)
                    pe.DictionaryAdd(key, Request.Form[key]);
                pe.HttpMethod = "post";

                var ret = ScriptModel.Run(name, pe);
                if (ret.StartsWith("REDIRECT="))
                    return Redirect(ret.Substring(9).trim());
                return Content(ret);
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }

        [HttpPost, Route("~/PyScriptForm")]
        public ActionResult PyScriptForm()
        {
            try
            {
                var pe = new PythonModel(Util.Host);
                foreach (var key in Request.Form.AllKeys)
                    pe.DictionaryAdd(key, Request.Form[key]);
                pe.HttpMethod = "post";

                var script = DbUtil.Db.ContentOfTypePythonScript(pe.Data.pyscript);
                return Content(pe.RunScript(script));
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }

    }

}
