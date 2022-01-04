﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Controllers;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Login;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
    public class LoginTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        readonly HttpClient client;

        public LoginTest(WebApplicationFactory<Startup> fixture)
        {
            client = fixture.CreateClient();  //vytvori se klient
        } 

        [Fact]
        public void LoginTestReturnOkAndToken()
        {
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Nya", Password = "catcatcat" }); //vezmu json, ktery tam chci poslat akonvert. Serialize je z norm objektu do jsonu
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json"); //udelam z toho spravnou klacuu jaky to ma enchoding; app/json je typ souboru
            
            var response = client.PostAsync("http://localhost:20625/login", requestContent).Result; //smazat. dělá to samé co ř.35
            string respond = response.Content.ReadAsStringAsync().Result;                              //ještě jednou na to čekám
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(respond);                
            var resultFromToken = token.Token.Split('.');
            var result = resultFromToken[0];

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Ok", token.Status);
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", result);
        }

        [Fact]
        public void LoginTestWithEmptyUserNameOrPassword()
        {
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Nya", Password = ""});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:20625/login", requestContent).Result;
            string respond = response.Content.ReadAsStringAsync().Result;
            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(respond);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Field username and/or field password was empty!", error.Error);
        }

        [Fact]
        public void LoginTestWithWrongUserOrPassword()
        {
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Nya", Password = "catcatcar" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:20625/login", requestContent).Result;
            string respond = response.Content.ReadAsStringAsync().Result;
            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(respond);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Username and/or password was incorrect!", error.Error);
        }


    }
}
