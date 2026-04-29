namespace EmployeeManagementSystem.Models.Dtos


{
    public class ResetPasswordRequestDto

    {
      
      public string Token { get; set; }  = "";
      public string Email { get; set; }  = "";
       public string TokenId { get; set; } = "";
      public string Password { get; set; }  = "";

      
    }

}