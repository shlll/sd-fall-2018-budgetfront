using BudgetFrontEnd.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BudgetFrontEnd.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("Email", model.Email));
            parameters.Add(
                new KeyValuePair<string, string>("Password", model.Password));
            parameters.Add(
                new KeyValuePair<string, string>("ConfirmPassword", model.ConfirmPassword));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            var response = httpClient
                .PostAsync("http://localhost:64310/api/account/register",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Message"] = "Your account has been created successfully!";
                return RedirectToAction("Login");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<ApiError>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else
            {
                //Create a log for the error message

                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("username", model.Email));
            parameters.Add(
                new KeyValuePair<string, string>("Password", model.Password));
            parameters.Add(
                new KeyValuePair<string, string>("grant_type", "password"));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            var response = httpClient
                .PostAsync("http://localhost:64310/token",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var info = JsonConvert.DeserializeObject<LoginInfo>(data);

                var cookie = new HttpCookie("BudgetA", info.AccessToken);

                Response.Cookies.Add(cookie);

                return RedirectToAction("Index", "Household");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var error = JsonConvert.DeserializeObject<AuthError>(data);

                ModelState.AddModelError("", error.ErrorDescription);

                return View(model);
            }
            else
            {
                //Create a log for the error message

                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            var cookie = Request.Cookies["BudgetA"];

            if (cookie == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var cookie = Request.Cookies["BudgetA"];

            if (cookie == null)
            {
                return RedirectToAction("Login");
            }

            var token = cookie.Value;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("OldPassword", model.OldPassword));
            parameters.Add(
                new KeyValuePair<string, string>("NewPassword", model.NewPassword));
            parameters.Add(
                new KeyValuePair<string, string>("ConfirmPassword", model.ConfirmPassword));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient
                .PostAsync("http://localhost:64310/api/account/changepassword",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ViewBag.Message = "Password has been changed succesfully";
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<ApiError>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else
            {
                //Create a log for the error message

                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            var cookie = new HttpCookie("BudgetA", "");
            cookie.Expires = DateTime.Now.AddDays(-10);
            
            Response.Cookies.Add(cookie);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("Email", model.Email));
            
            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();
            
            var response = httpClient
                .PostAsync("http://localhost:64310/api/account/forgotpassword",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("ForgotPasswordSuccess");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<ApiError>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else
            {
                //Create a log for the error message

                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {  
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("Email", model.Email));
            parameters.Add(
                new KeyValuePair<string, string>("Password", model.Password));
            parameters.Add(
                new KeyValuePair<string, string>("ConfirmPassword", model.ConfirmPassword));

            parameters.Add(
                new KeyValuePair<string, string>("Code", model.Code));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            var response = httpClient
                .PostAsync("http://localhost:64310/api/account/resetpassword",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("ResetPasswordSuccess");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<ApiError>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else
            {
                //Create a log for the error message

                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }
    }
}