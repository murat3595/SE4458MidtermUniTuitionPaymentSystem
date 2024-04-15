using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuitionApi.Models.Dto;

namespace TuitionApi.Controllers.V1
{

    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/Tuition")]
    [ControllerName("Tuition Version 1.0")]
    [ApiController]
    public class TuitionController : ControllerBase
    {
        private DummyDataContext _dummyDataContext;
        public TuitionController(DummyDataContext dummyDataContext)
        {
            this._dummyDataContext = dummyDataContext;
        }

        /// <summary>
        /// Get users tuition and balance with his/her profile information 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getTuitionOfStudent")]
        public IActionResult GetTuition([FromQuery] int id)
        {
            var result = _dummyDataContext.Users
                .Where(s => s.UserId == id)
                .Select(k => new TuitionDto
                    {
                        Balance = k.Balance,
                        Tuition = k.TuitionTotal,
                        User = new UserDto
                        {
                            Name = k.Name,
                            Surname = k.Surname,
                        }
                    })
                .FirstOrDefault();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Send payment amount with userId. You will get a paymentStatus success result if it was succesful. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("payTuitionOfStudent")]
        public IActionResult PayTuition([FromBody] PayTuitionRequestDto request)
        {
            var user = _dummyDataContext.Users.FirstOrDefault(s => s.UserId == request.UserID);

            if (user == null)
                return NotFound(new
                {
                    PaymentStatus = "Error"
                });

            user.Balance += request.PaymentAmount;

            _dummyDataContext.SaveChanges();

            return Ok (new
            {
                PaymentStatus = "Success"
            });
        }

        /// <summary>
        /// To add tuition to a user send additional tuition with userId. This can be used in 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("addTuitionForStudent")]
        public IActionResult AddTuitionToUser([FromBody] AddTuitionToUserRequest request)
        {
            var user = _dummyDataContext.Users.FirstOrDefault(s => s.UserId == request.UserID);

            if (user == null)
                return NotFound(new
                {
                    TransactionStatus = "error",
                });

            user.TuitionTotal += request.AdditionalTuition;


            return Ok(new
            {
                TransactionStatus = "success",
                Tuition = new TuitionDto
                {
                    Balance = user.Balance,
                    Tuition = user.TuitionTotal,
                    User = new UserDto
                    {
                        Name = user.Name,
                        Surname = user.Surname,
                    }
                }
            });
        }

        /// <summary>
        /// This will return only the students who haven't paid their tuition yet. isThereMore field is telling you if there is more pages.
        /// </summary>
        /// <param name="elementCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("GetUnpaidStudents")]
        [Authorize]
        public IActionResult GetUnpaidUsers([FromQuery] int elementCount, [FromQuery] int pageIndex)
        {
            var result = _dummyDataContext.Users
                .Where(s => s.Balance < s.TuitionTotal)
                .Skip(elementCount * pageIndex)
                .Take(elementCount + 1)
                .Select(s => new TuitionDto
                {
                    Balance = s.Balance,
                    Tuition = s.TuitionTotal,
                    User = new UserDto { Name = s.Name, Surname = s.Surname,}
                }).ToList();

            var isThereMore = result.Count > elementCount;

            if (isThereMore)
            {
                result = result.Take(elementCount).ToList();
            }

            return Ok(new
            {
                IsThereMore = isThereMore,
                Students = result
            });
        }


    }
}
