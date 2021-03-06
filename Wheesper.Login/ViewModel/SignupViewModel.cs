﻿using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using Wheesper.Login.events;
using Wheesper.Login.Model;

using Wheesper.Messaging.events;
using ProtocolBuffer;
using System.Diagnostics;
using Wheesper.Infrastructure.events;

namespace Wheesper.Login.ViewModel
{
    public class SignupViewModel : NotificationObject
    {
        private IUnityContainer container = null;
        private IEventAggregator eventAggregator = null;
        private LoginModel loginModel = null;

        public void Initialize(string email)
        {
            loginModel.clearAllField();
            Email = email;
        }

        #region helper function
        private void subevent()
        {
            eventAggregator.GetEvent<MsgSignupMailResponseEvent>().Subscribe(SignupMailResponseEventHandler);
            eventAggregator.GetEvent<MsgSignupInfoResponseEvent>().Subscribe(SignupInfoResponseEventHandler);
        }
        #endregion helper function

        #region Constructor & deconstructor
        public SignupViewModel(IUnityContainer container, LoginModel loginModel)
        {
            Debug.WriteLine("SignupViewModel constructor");
            this.container = container;
            eventAggregator = this.container.Resolve<IEventAggregator>();
            this.loginModel = loginModel;

            subevent();
        }

        ~SignupViewModel()
        {
            Debug.WriteLine("SignupViewModel deconstructor");
        }
        #endregion Constructor & deconstructor

        #region helper variable
        private string emailInvalidMessage = "你输入的邮箱格式不正确.";
        private string emailexistMessage = "你输入的邮箱已经被注册. 如果这个邮箱是你的，请直接登陆.";
        private string pwInvalidMessage = "Passwords must have at least 8 characters and contain at least two of the following: uppercase letters, lowercase letters, numbers, and symbols.";
        private string captchaInvalidMessage = "Please enter the 4-digit code. The code only contains numbers.";
        private string captchaWrongMessage = "你输入的验证码不正确，请确认后再次输入.";
        #endregion helper variable

        #region properties
        private string uiInfo = "Enter the password for ";
        private string promtInfo = null; // email wrong, pw wrong
        public string UIInfo
        {
            get
            {
                return uiInfo;
            }
            set
            {
                uiInfo = value;
                RaisePropertyChanged("UIInfo");
            }
        }
        public string PromtInfo
        {
            get
            {
                return promtInfo;
            }
            set
            {
                promtInfo = value;
                RaisePropertyChanged("PromtInfo");
            }
        }
        public string Email
        {
            get
            {
                return loginModel.email;
            }
            set
            {
                loginModel.email = value;
                RaisePropertyChanged("EMail");
                SignupInfoNextCommand.RaiseCanExecuteChanged();
            }
        }
        public string Password
        {
            get
            {
                return loginModel.password;
            }
            set
            {
                loginModel.password = value;
                RaisePropertyChanged("Password");
                SignupInfoNextCommand.RaiseCanExecuteChanged();
            }
        }
        public string Nickname
        {
            get
            {
                return loginModel.nickname;
            }
            set
            {
                loginModel.nickname = value;
                RaisePropertyChanged("NickName");
                SignupDetailsNextCommand.RaiseCanExecuteChanged();
            }
        }
        public string Captcha
        {
            get
            {
                return loginModel.captcha;
            }
            set
            {
                loginModel.captcha = value;
                RaisePropertyChanged("Captcha");
                SignupCaptchaNextCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion proterties

        #region command
        private DelegateCommand signupInfoNextCommand;
        private DelegateCommand signupInfoBackCommand;
        private DelegateCommand signupDetailsNextCommand;
        private DelegateCommand signupDetailsBackCommand;
        private DelegateCommand signupCaptchaNextCommand;
        private DelegateCommand signupCaptchaBackCommand;

        public DelegateCommand SignupInfoNextCommand
        {
            get
            {
                if (signupInfoNextCommand == null)
                {
                    signupInfoNextCommand = new DelegateCommand(signupInfoNext, canSignupInfoNext);
                }
                return signupInfoNextCommand;
            }
            set
            {
                signupInfoNextCommand = value;
            }
        }

        public DelegateCommand SignupInfoBackCommand
        {
            get
            {
                if (signupInfoBackCommand == null)
                {
                    signupInfoBackCommand = new DelegateCommand(signupInfoBack, canSignupInfoBack);
                }
                return signupInfoBackCommand;
            }
            set
            {
                signupInfoBackCommand = value;
            }
        }

        public DelegateCommand SignupDetailsNextCommand
        {
            get
            {
                if (signupDetailsNextCommand == null)
                {
                    signupDetailsNextCommand = new DelegateCommand(signupDetailsNext, canSignupDetailsNext);
                }
                return signupDetailsNextCommand;
            }
            set
            {
                signupDetailsNextCommand = value;
            }
        }

        public DelegateCommand SignupDetailsBackCommand
        {
            get
            {
                if (signupDetailsBackCommand == null)
                {
                    signupDetailsBackCommand = new DelegateCommand(signupDetailsBack, canSignupDetailsBack);
                }
                return signupDetailsBackCommand;
            }
            set
            {
                signupDetailsBackCommand = value;
            }
        }

        public DelegateCommand SignupCaptchaNextCommand
        {
            get
            {
                if (signupCaptchaNextCommand == null)
                {
                    signupCaptchaNextCommand = new DelegateCommand(signupCaptchaNext, canSignupCaptchaNext);
                }
                return signupCaptchaNextCommand;
            }
            set
            {
                signupCaptchaNextCommand = value;
            }
        }

        public DelegateCommand SignupCaptchaBackCommand
        {
            get
            {
                if (signupCaptchaBackCommand == null)
                {
                    signupCaptchaBackCommand = new DelegateCommand(signupCaptchaBack, canSignupCaptchaBack);
                }
                return signupCaptchaBackCommand;
            }
            set
            {
                signupCaptchaBackCommand = value;
            }
        }
        #endregion command

        #region Command Delegate Method
        private void signupInfoNext()
        {
            if (!loginModel.isEmailAddress(Email))
            {
                // TODO: 
                PromtInfo = emailInvalidMessage;
                return;
            }
            else if(!loginModel.isPWQualified(Password))
            {
                // TODO: 
                PromtInfo = pwInvalidMessage;
            }
            else
            {
                // TODO: sub should enter an animate_2 to avoid user do anything
                PromtInfo = null;
                loginModel.sendSignupMailRequest();
            }
        }
        private bool canSignupInfoNext()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void signupInfoBack()
        {
            loginModel.currentState = State.SIGNIN;
            Debug.Write("Client State change to ");
            Debug.WriteLine(loginModel.currentState.ToString());
            eventAggregator.GetEvent<SignupInfoBackEvent>().Publish(0);
        }
        private bool canSignupInfoBack()
        {
            return true;
        }

        private void signupDetailsNext()
        {
            loginModel.sendCaptchaRequest();
            UIInfo = uiInfo.Insert(uiInfo.Length, Email);
            eventAggregator.GetEvent<SignupDetailsNextEvent>().Publish(0);
        }
        private bool canSignupDetailsNext()
        {
            return !string.IsNullOrWhiteSpace(Nickname);
        }

        private void signupDetailsBack()
        {
            Nickname = null;
            eventAggregator.GetEvent<SignupDetailsBackEvent>().Publish(0);
        }
        private bool canSignupDetailsBack()
        {
            return true;
        }

        private void signupCaptchaNext()
        {
            if (loginModel.isCaptchaQualified(Captcha))
            {
                // TODO: sub should enter an animate_1 to avoid user do anything
                loginModel.sendSignupInfoRequest();
            }
            else
            {
                PromtInfo = captchaInvalidMessage;
            }
        }
        private bool canSignupCaptchaNext()
        {
            return !string.IsNullOrWhiteSpace(Captcha);
        }

        private void signupCaptchaBack()
        {
            Captcha = null;
            eventAggregator.GetEvent<SignupCaptchaBackEvent>().Publish(0);
        }
        private bool canSignupCaptchaBack()
        {
            return true;
        }
        #endregion Command Delegate Method

        #region event handler
        private void SignupMailResponseEventHandler(ProtoMessage message)
        {
            if (loginModel.currentState != State.SIGNUP)
                return;
            bool status = message.SignupMailResponse.Status;
            if (status)
            {
                // TODO: exit animate_2 and enter signup details view
                eventAggregator.GetEvent<SignupInfoNextEvent>().Publish(0);
            }
            else
            {
                // TODO: exit animate_2 and return signup info view
                PromtInfo = emailexistMessage;
            }
        }

        private void SignupInfoResponseEventHandler(ProtoMessage message)
        {
            bool status = message.SignupInfoResponse.Status;
            if (status)
            {
                // TODO: exit animate_1 and enter welcome
                //loginModel.sendSigninRequest();
                //eventAggregator.GetEvent<LoginEvent>().Publish(Email);
                eventAggregator.GetEvent<SignupCaptchaNextEvent>().Publish(Nickname);
            }
            else
            {
                // TODO: exit animate_1 and return signup captcha view
                PromtInfo = captchaWrongMessage;
            }
        }
        #endregion event handler
    }
}
