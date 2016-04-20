﻿using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.API;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public int CurrentOrgId { get; set; }

        public void AddMembersToOrg(object query, object orgId)
        {
            var q = db.PeopleQuery2(query);
            var dt = DateTime.Now;
            foreach (var p in q)
            {
                var db2 = NewDataContext();
                OrganizationMember.InsertOrgMembers(db2, orgId.ToInt(), p.PeopleId, MemberTypeCode.Member, dt, null, false);
                db2.Dispose();
            }
        }

        public void AddMemberToOrg(object pid, object orgId)
        {
            AddMembersToOrg(pid.ToInt(), orgId.ToInt());
        }

        public void AddSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                throw new Exception($"no orgmember {pid}:");
            var db2 = NewDataContext();
            om.AddToGroup(db2, group);
            db2.Dispose();
        }

        public int FindOrgForAgeMonths(int divid, int? monthsOld)
        {
            if (!monthsOld.HasValue)
                return 0;
            var q = from o in db.Organizations
                    let smos = o.OrganizationExtras.FirstOrDefault(vv => vv.Field == "StartMonths").IntValue
                    let emos = o.OrganizationExtras.FirstOrDefault(vv => vv.Field == "EndMonths").IntValue
                    where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    where smos <= monthsOld && emos >= monthsOld
                    select o.OrganizationId;
            return q.FirstOrDefault();
        }

        public int FindOrgForBirthdate(int divid, DateTime? birthdate)
        {
            if (!birthdate.HasValue)
                return 0;
            var q = from o in db.Organizations
                    where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    where o.BirthDayStart <= birthdate
                    where o.BirthDayEnd >= birthdate
                    select o.OrganizationId;
            return q.FirstOrDefault();
        }

        public APIOrganization.Organization GetOrganization(object orgId)
        {
            var api = new APIOrganization(db);
            return api.GetOrganization(orgId.ToInt());
        }

        public bool InOrg(object pid, object orgId)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            return om != null;
        }

        public bool InSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                return false;

            return om.IsInGroup(group);
        }

        public void JoinOrg(int orgid, object person)
        {
            var db2 = NewDataContext();

            if (person is int)
                OrganizationMember.InsertOrgMembers(db2, orgid, person.ToInt(), 220, DateTime.Now, null, false);
            else if (person is Person)
                OrganizationMember.InsertOrgMembers(db2, orgid, ((Person) person).PeopleId, 220, DateTime.Now, null, false);

            db2.Dispose();
        }

        public void MoveToOrg(int pid, int fromOrg, int toOrg)
        {
            var db2 = NewDataContext();
            if (fromOrg == toOrg)
                return;
            var om = db2.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == fromOrg);
            var sg = om.OrgMemMemTags.Select(mt => mt.MemberTag.Name).ToList();
            var tom = db2.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == toOrg);
            if (tom == null)
            {
                tom = OrganizationMember.InsertOrgMembers(db2,
                    toOrg, pid, MemberTypeCode.Member, om.EnrollmentDate ?? DateTime.Now, om.InactiveDate, om.Pending ?? false);
                if (tom == null)
                    return;
            }
            tom.Request = om.Request;
            tom.Amount = om.Amount;
            tom.UserData = om.UserData;
            tom.OnlineRegData = om.OnlineRegData;
            tom.RegistrationDataId = om.RegistrationDataId;
            tom.Grade = om.Grade;
            tom.RegisterEmail = om.RegisterEmail;
            tom.MemberTypeId = om.MemberTypeId;
            tom.ShirtSize = om.ShirtSize;
            tom.TranId = om.TranId;
            tom.Tickets = om.Tickets;
            foreach (var s in sg)
                tom.AddToGroup(db2, s);
            if (om.OrganizationId != tom.OrganizationId)
                tom.Moved = true;
            om.Drop(db2);
            db2.SubmitChanges();
        }

        public List<int> OrganizationIds(int progid, int divid, bool? includeInactive)
        {
            var q = from o in db.Organizations
                    where progid == 0 || o.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                    where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                    where includeInactive == true || o.OrganizationStatusId == OrgStatusCode.Active
                    select o.OrganizationId;
            return q.ToList();
        }

        public void RemoveSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                throw new Exception($"no orgmember {pid}:");
            var db2 = NewDataContext();
            om.RemoveFromGroup(db2, group);
            db2.Dispose();
        }

        public void UpdateMainFellowship(int orgId)
        {
            var db2 = NewDataContext();
            db2.UpdateMainFellowship(orgId);
        }
    }
}