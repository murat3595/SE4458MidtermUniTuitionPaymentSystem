namespace TuitionApi.Models.Dto
{
    public class AddTuitionToUserRequest
    {
        public int UserID { get; set; }
        public float AdditionalTuition { get; set; }
    }
}
