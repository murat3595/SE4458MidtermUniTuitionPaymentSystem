using TuitionApi.Models.Dto;

public class UserLoginDtoRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginDtoResponse
    {
        public string Token { get; set; }

        public UserDto User { get; set; }
    }