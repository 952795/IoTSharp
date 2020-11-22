﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IoTSharp.Sdk.Http
{
    public class IoTSharpClient
    {
        public IoTSharpClient()
        {


        }
        public IoTSharpClient(Uri uri)
        {
            BaseURL = uri.ToString();
        }
        public bool Anonymous => (_result?.Roles?.Contains(nameof(UserRole.Anonymous))).GetValueOrDefault();
        public bool CustomerAdmin => (_result?.Roles?.Contains(nameof(UserRole.CustomerAdmin))).GetValueOrDefault();
        public bool NormalUser => (_result?.Roles?.Contains(nameof(UserRole.NormalUser))).GetValueOrDefault();
        public bool SystemAdmin => (_result?.Roles?.Contains(nameof(UserRole.SystemAdmin))).GetValueOrDefault();
        public bool TenantAdmin => (_result?.Roles?.Contains(nameof(UserRole.TenantAdmin))).GetValueOrDefault();
        public TokenEntity Token => _result.Token;
        public bool IsLogin => (_result?.Succeeded).GetValueOrDefault();
        public bool CanLogout => IsLogin;

        public bool CanLogin => !IsLogin;

        public UserInfoDto MyInfo { get; set; }

        public string BaseURL { get; set; } = "http://localhost:51498";
        public HttpClient HttpClient { get; set; } = new HttpClient();

        public T Create<T>() where T : class
        {
            T t = Activator.CreateInstance(typeof(T), HttpClient) as T;
            typeof(T).GetProperty("BaseUrl").SetValue(t, BaseURL);
            return t;
        }
        AccountClient _act_client;
        private LoginResult _result;

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                _act_client = new AccountClient(HttpClient);
                _result = await _act_client.LoginAsync(new LoginDto() { UserName = username, Password = password });
                HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.Access_token}");
                ApiResultOfUserInfoDto userInfoDto = await _act_client.MyInfoAsync();
                MyInfo = userInfoDto.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("Login error ", ex);
            }
            return (bool)(_result?.SignIn.Succeeded);
        }
        public async void LogoutAsync()
        {
            await _act_client.LogoutAsync();
        }
    }
}
