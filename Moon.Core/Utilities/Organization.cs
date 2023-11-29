using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;

namespace Moon.Core.Utilities
{
    public class Organization
    {
        #region IsManagementInfiniteLoop
        public static bool IsManagementInfiniteLoop(string owner, int organization)
        {
            return IsManagementInfiniteLoop(owner, organization, 0, 100);
        }
        private static bool IsManagementInfiniteLoop(string owner, int organization, int layer, int maxLayer)
        {
            Organizations organizations = Database.Edgerunners.Queryable<Organizations>().First(it => it.Id == organization);
            if (organizations == null)
                throw new Exception($"[Recursion] Invalid Organization ({organization}) , please check and try again");
            if (layer == maxLayer)
                throw new Exception($"[Recursion] Reached maximum recursion count ({maxLayer})");
            layer++;
            Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == owner);
            if (user == null)
                throw new Exception($"[Recursion] Invalid Owner ({owner}) , please check and try again");
            if (user.Organization == null)
                return false;
            if (user.Organization == organization)
                return true;
            else
            {
                Organizations org = Database.Edgerunners.Queryable<Organizations>().First(it => it.Id == user.Organization);
                return IsManagementInfiniteLoop(org.Owner, organization, layer, maxLayer);
            }
        }
        #endregion

        #region IsBeManagementInfiniteLoop
        public static bool IsBeManagementInfiniteLoop(string member, int distOrganization)
        {
            return IsBeManagementInfiniteLoop(member, distOrganization, 0, 100);
        }
        private static bool IsBeManagementInfiniteLoop(string member, int distOrganization, int layer, int maxLayer)
        {
            Organizations organizations = Database.Edgerunners.Queryable<Organizations>().First(it => it.Id == distOrganization);
            if (organizations == null)
                throw new Exception($"[Recursion] Invalid Organization ({distOrganization}) , please check and try again");
            if (organizations.Owner == member)
                return true;
            if (layer == maxLayer)
                throw new Exception($"[Recursion] Reached maximum recursion count ({maxLayer})");
            layer++;
            Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == member);
            if (user == null)
                throw new Exception($"[Recursion] Invalid Member ({member}) , please check and try again");
            Organizations org = Database.Edgerunners.Queryable<Organizations>().First(it => it.Owner == user.EmployeeId);
            if (org == null)
                return false;
            Users userx = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == organizations.Owner);
            if (userx.Organization == null)
                return false;
            if (org.Id == userx.Organization)
                return true;
            return IsBeManagementInfiniteLoop(member, (int)userx.Organization, layer, maxLayer);
        }
        #endregion

        #region GetOrganizationChart
        public class OrganizationChart
        {
            public string EmployeeId { get; set; }
            public string Name { get; set; }
            public string? ItCode { get; set; }
            public string Position { get; set; }
            public string? Organization { get; set; }
            public List<OrganizationChart> Children { get; set; } = new();
        }
        public class OrganizationChartFlat
        {
            public string Id { get; set; }
            public string? ItCode { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string? Organization { get; set; }
            public string? ParentId { get; set; }
        }
        public static List<OrganizationChartFlat> GetOrganizationChartFlat()
        {
            List<OrganizationChartFlat> orgCharts = Database.Edgerunners.Queryable<Users>().LeftJoin<Organizations>((u, o) => u.Organization == o.Id).Select((u, o) => new OrganizationChartFlat
            {
                Id = u.EmployeeId,
                ItCode = u.ItCode,
                Name = u.Name,
                Position = u.Position,
                Organization = o.Name,
                ParentId = o.Owner
            }).ToList();
            foreach (OrganizationChartFlat orgChart in orgCharts)
            {
                if (orgChart.ParentId == null)
                    orgChart.ParentId = "root";
            }
            orgCharts.Add(new OrganizationChartFlat
            {
                Id = "root",
                ItCode = string.Empty,
                Name = "TECC",
                Position = "Test Engineering Control Center",
                ParentId = null
            });
            return orgCharts;
        }
        #endregion

        #region GetSubordinates

        public static List<Users> GetSubordinates(string startPoint)
        {
            Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == startPoint);
            if (user == null)
                throw new Exception($"Invalid employeeId ({startPoint}) , please re login");
            List<Users> subordinates = GetSubordinates(startPoint, new List<Users>(), 0, 100);
            subordinates.Add(user);
            return subordinates;
        }
        private static List<Users> GetSubordinates(string startPoint, List<Users> subordinat, int layer, int maxLayer)
        {
            if (layer == maxLayer)
                throw new Exception($"[Recursion] Reached maximum recursion count ({maxLayer})");
            layer++;
            Organizations org = Database.Edgerunners.Queryable<Organizations>().First(it => it.Owner == startPoint);
            if (org != null)
            {
                List<Users> users = Database.Edgerunners.Queryable<Users>().Where(it => it.Organization == org.Id).ToList();
                foreach (var user in users)
                {
                    subordinat.Add(user);
                    GetSubordinates(user.EmployeeId, subordinat, layer, maxLayer);
                }

            }
            return subordinat;
        }
        #endregion
    }
}
