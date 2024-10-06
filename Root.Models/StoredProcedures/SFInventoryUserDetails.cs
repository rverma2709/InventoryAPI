using System.ComponentModel;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("InventoryUserDetails")]
    public class SFInventoryUserDetails
    {
        [QueryParam]
        public string? UserName { get; set; }
        [QueryParam]
        public string? Password { get; set; }
        [QueryParam(direction: Direction.Output)]
        public bool IsSuccess { get; set; }
    }
    public class SFUserData
    {
        public long InventoryUserId { get; set; }
        public long InventoryRoleId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactNo { get; set; }
        public string? ImagePath { get; set; }
        public string? InventoryPermissionIds {  get; set; }

    }
}
