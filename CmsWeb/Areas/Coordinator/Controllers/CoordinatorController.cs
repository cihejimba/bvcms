using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Coordinator.Models;
using Community.CsharpSqlite;
using UtilityExtensions;

namespace CmsWeb.Areas.Coordinator.Controllers
{
    public class CoordinatorController : Controller
    {
        public static List<ProgModel> Programs;

        public static ProgViewModel pvm = new ProgViewModel();
        public static DivViewModel dvm = new DivViewModel();
        public static OrgViewModel ovm = new OrgViewModel();

        public ActionResult ProgdivOrg()
        {
            Progs();
            return View();
        }

        public ActionResult DivisionView(int? progId)
        {
            dvm.Divlist.Clear();
            if (progId != null)
            {
                ProgModel cd = Programs.Find(p => p.Id == progId);

                foreach (DivModel spd in cd.DivList)
                {
                    dvm.Divlist.Add(spd);
                }
            }
            return View(dvm);
        }

        public ActionResult ProgramView()
        {
            pvm.Proglist.Clear();

            foreach (ProgModel pd in Programs)
            {
                pvm.Proglist.Add(pd);
            }
            return View(pvm);
        }

        public ActionResult OrganizationView(int? progId, int? divId)
        {
            ovm.Orglist.Clear();
            if (progId != null) // && divId != null)
            {

                ProgModel cd = Programs.Find(p => p.Id == progId);
                DivModel spd = cd.DivList.Find(p => p.Id == divId);
                if (divId.IsNull())
                {
                    foreach (var x in cd.DivList)
                    {
                        foreach (OrgModel cpd in x.OrgList)
                        {
                            ovm.Orglist.Add(cpd);
                        }
                    }
                }
                else
                {
                    foreach (OrgModel cpd in spd.OrgList)
                    {
                        ovm.Orglist.Add(cpd);
                    }
                }

            }
            return View(ovm);
        }

        public ActionResult Index(int id)
        {
            var m = new SubgroupModel(id);
            return View(m);
        }

        public ActionResult SubgroupDataView(int id, int grpid, string sg)
        {
            var m = new SubgroupModel(id);
            m.ingroup = sg;
            m.groupid = grpid;
            return View(m);
        }

        public static void Progs()
        {
            // TODO: limit selection to programs capable of self checkin 
            List<ProgModel> pList = (from program in DbUtil.Db.Programs
                                     join pd in DbUtil.Db.ProgDivs on program.Id equals pd.ProgId
                                     join division in DbUtil.Db.Divisions on pd.DivId equals division.Id
                                     join organization in DbUtil.Db.Organizations on division.Id equals organization.DivisionId
                                     join mt in DbUtil.Db.MemberTags on organization.OrganizationId equals mt.OrgId
                                     where organization.CanSelfCheckin == true
                                     && mt.CheckIn == true
                                     select new ProgModel
                                     {
                                         Id = program.Id,
                                         ProgName = program.Name,
                                         DivList = Divisions(program.Id)
                                     }).Distinct().ToList();
            Programs = pList;
        }

        public static List<DivModel> Divisions(int progId)
        {
            List<DivModel> divisionList = (from program in DbUtil.Db.Programs
                                           join pd in DbUtil.Db.ProgDivs on program.Id equals pd.ProgId
                                           join division in DbUtil.Db.Divisions on pd.DivId equals division.Id
                                           join organization in DbUtil.Db.Organizations on division.Id equals organization.DivisionId
                                           join mt in DbUtil.Db.MemberTags on organization.OrganizationId equals mt.OrgId
                                           where organization.CanSelfCheckin == true
                                           && mt.CheckIn == true
                                           && program.Id == progId
                                           select new DivModel
                                           {
                                               Id = division.Id,
                                               DivName = division.Name,
                                               OrgList = Organizations(division.Id)
                                           }).Distinct().ToList();

            return divisionList;
        }

        public static List<OrgModel> Organizations(int divId)
        {
            List<OrgModel> orgList = (from program in DbUtil.Db.Programs
                                      join pd in DbUtil.Db.ProgDivs on program.Id equals pd.ProgId
                                      join division in DbUtil.Db.Divisions on pd.DivId equals division.Id
                                      join organization in DbUtil.Db.Organizations on division.Id equals organization.DivisionId
                                      join mt in DbUtil.Db.MemberTags on organization.OrganizationId equals mt.OrgId
                                      where organization.CanSelfCheckin == true
                                      && mt.CheckIn == true
                                      && division.Id == divId
                                      select new OrgModel()
                                      {
                                          Id = organization.OrganizationId,
                                          OrgName = organization.OrganizationName,
                                          Capacity = organization.Limit ?? 0,
                                          Attendance = organization.Attends.Count
                                      }).Distinct().ToList();

            return orgList;
        }


        public ActionResult SubgroupView(SubgroupModel m)
        {
            return View(m);
        }

        public ActionResult MoveSubgroupView(int id, int grpid, string list)
        {
            var m = new SubgroupModel(id);
            m.groupid = grpid;
            string[] arr = list.Split(',');
            int[] selectedIds = Array.ConvertAll(arr, int.Parse);           
            m.SelectedPeopleIds = selectedIds;
            return View(m);
        }

        [HttpPost]
        public ActionResult UpdateCapacity(PostTargetInfo i)
        {
            var smgroup = (from e in DbUtil.Db.MemberTags
                           where e.OrgId == i.id
                           where e.Id == i.grpid
                           select e).SingleOrDefault();

            if (smgroup != null)
            {
                int oldcapacity = smgroup.CheckInCapacity;
                if (i.addremove == 1)
                    smgroup.CheckInCapacity = oldcapacity + 1;
                else
                    smgroup.CheckInCapacity = smgroup.CheckInCapacity - 1;

                if (oldcapacity <= smgroup.CheckInCapacity)
                    smgroup.CheckInOpen = false;
            }
            DbUtil.Db.SubmitChanges();
            var m = new SubgroupModel(i.id);
            m.groupid = i.grpid;
            m.ingroup = m.GetGroupDetails(i.grpid).Name;
            return View("SubgroupDataView", m);
        }

        [HttpPost]
        public ActionResult UpdateCheckInOpen(PostTargetInfo i)
        {
            var smgroup = (from e in DbUtil.Db.MemberTags
                where e.OrgId == i.id
                where e.Id == i.grpid
                select e).SingleOrDefault();

            if (smgroup != null)
            {
                if (i.addremove == 1)
                    smgroup.CheckInOpen = true;
                else
                    smgroup.CheckInOpen = false;

            }
            DbUtil.Db.SubmitChanges();
            var m = new SubgroupModel(i.id);
            m.groupid = i.grpid;
            m.ingroup = m.GetGroupDetails(i.grpid).Name;
            return View("SubgroupDataView", m);
        }

        public ActionResult UpdateSmallGroup(int id, int curgrpid, int targrpid, string list)
        {
            string[] arr = list.Split(',');
            int[] selectedIds = Array.ConvertAll(arr, int.Parse);
            var m = new SubgroupModel(id);
            var a = selectedIds;

            //Add members to subgroup
            var tarsgname = DbUtil.Db.MemberTags.Single(mt => mt.Id == targrpid).Name;
            var cursgname = DbUtil.Db.MemberTags.Single(mt => mt.Id == curgrpid).Name;
            var q2 = from om in m.OrgMembers()
                where om.OrgMemMemTags.All(mt => mt.MemberTag.Id == curgrpid)
                where a.Contains(om.PeopleId)
                select om;
            foreach (var om in q2)
            {                
                om.AddToGroup(DbUtil.Db, tarsgname);
                om.RemoveFromGroup(DbUtil.Db, cursgname);
            }
            DbUtil.Db.SubmitChanges();

            m.groupid = targrpid;
            m.ingroup = m.GetGroupDetails(targrpid).Name;
            return RedirectToAction("SubgroupView", m);
        }
    }

    public class PostTargetInfo
    {
        public int id { get; set; }
        public int grpid { get; set; }
        public int addremove { get; set; }

    }
}
