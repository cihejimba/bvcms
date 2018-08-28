using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace CmsData
{
    public static class ContributionFundExtensions
    {
        public static IQueryable<ContributionFund> ScopedByRoleMembership(this IQueryable<ContributionFund> contributionFunds)
        {
            return contributionFunds.ScopedByRoleMembership(Roles.GetRolesForUser());
        }

        public static IQueryable<ContributionFund> ScopedByRoleMembership(this IQueryable<ContributionFund> contributionFunds, string[] allowedRoles)
        {
            if (allowedRoles != null)
            {
                // get list of role names used as fund manager roles
                var limitingRoles = contributionFunds
                    .Join(DbUtil.Db.Roles, fund => fund.FundManagerRoleId, role => role.RoleId, (fund, role) => new { fund, role })
                    .Where(x => x.fund.FundManagerRoleId != 0)
                    .Select(x => x.role.RoleName)
                    .Distinct();

                // having the finance role gives you access to any and every fund
                if (allowedRoles.Contains("Finance"))
                {
                    return contributionFunds;
                }

                // having the financeviewonly role is multipurpose, by itself you have everything... paired w/ any limiting role and its just those funds
                if (allowedRoles.Contains("FinanceViewOnly"))
                {
                    var hasLimitingRoles = limitingRoles.Intersect(allowedRoles).Any();

                    if (!hasLimitingRoles)
                    {
                        return contributionFunds;
                    }
                    else
                    {
                        return contributionFunds.Where(f => f.FundManagerRoleId != 0)
                            .Join(DbUtil.Db.Roles, f => f.FundManagerRoleId, r => r.RoleId, (f, r) => new { role = r, fund = f })
                            .Where(r => allowedRoles.Contains(r.role.RoleName))
                            .Select(r => r.fund);
                    }
                }
            }

            // didn't have any relevant role to manage funds so you wouldn't see anything in this list
            return GetEmptyList();
        }

        private static IQueryable<ContributionFund> GetEmptyList()
        {
            return new List<ContributionFund>().AsQueryable();
        }
    }
}
