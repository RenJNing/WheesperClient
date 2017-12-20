﻿using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using Wheesper.Login.events;
using Wheesper.Login.Model;

using System.Text.RegularExpressions;
using Wheesper.Messaging.events;
using ProtocolBuffer;
using System.Diagnostics;

namespace Wheesper.Login.ViewModel
{
    public class SigninViewModel : NotificationObject
    {
        private IUnityContainer container = null;
        private IEventAggregator eventAggregator = null;
        private LoginModel loginModel = null;

        //private Regex emailRgx = null;
        //private string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        public void Initialize(string email)
        {
            loginModel.clearAllField();
            Email = email;
        }

        #region private helper variable
        private string enterPW = "Enter the password for ";
        private string emailInvalidMessage = "The email address you entered isn's vaild.";
        private string pwWrongMessage = "Your account or password is incorrect. If you don't remember your password, ";
        private string networkFailMessage = "There's some problem with network. Please try again. If you continue to get this message, try again later.";
        // private bool signinRequeststate = false;
        #endregion helper variable

        #region helper function
        private void prepare()
        {
            Password = null;
            PromtInfo = null;
            UIInfo = enterPW.Insert(enterPW.Length, loginModel.email);
        }


        #endregion helper function

        #region Constructor
        public SigninViewModel(IUnityContainer container)
        {
            Debug.WriteLine("signinviewmodel con");
            this.container = container;
            eventAggregator = this.container.Resolve<IEventAggregator>();
            // loginModel = this.container.Resolve<LoginModel>();
            loginModel = (LoginModel)container.Resolve(typeof(LoginModel));

            eventAggregator.GetEvent<MsgSigninResponseEvent>().Subscribe(SigninResponseEventHandler, ThreadOption.UIThread);
        }
        #endregion Constructor

        #region properties
        private string uiInfo = null; 
        private string promtInfo = null; // email wrong, pw wrong
        public string Email
        {
            get
            {
                return loginModel.email;
            }
            set
            {
                loginModel.email = value;
                Debug.WriteLine(Email);
                RaisePropertyChanged("Email");
                SigninMailNextCommand.RaiseCanExecuteChanged();
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
                Debug.WriteLine(Password);
                Debug.WriteLine(Email);
                RaisePropertyChanged("Password");
                SigninPWNextCommand.RaiseCanExecuteChanged();
            }
        }
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
                RaisePropertyChanged("Promt");
            }
        }
        #endregion properties

        #region command
        private DelegateCommand signinMailNextCommand;
        private DelegateCommand signinPWNextCommand;
        private DelegateCommand signinPWBackCommand;
        private DelegateCommand createAccountCommand;
        private DelegateCommand forgetPWCommand;

        public DelegateCommand SigninMailNextCommand
        {
            get
            {
                if (signinMailNextCommand == null)
                {
                    signinMailNextCommand = new DelegateCommand(signinMailNext, canSigninMailNext);
                }
                return signinMailNextCommand;
            }
            set
            {
                signinMailNextCommand = value;
            }
        }

        public DelegateCommand SigninPWNextCommand
        {
            get
            {
                if (signinPWNextCommand == null)
                {
                    signinPWNextCommand = new DelegateCommand(signinPWNext, canSigninPWNext);
                }
                return signinPWNextCommand;
            }
            set
            {
                signinPWNextCommand = value;
            }
        }

        public DelegateCommand SigninPWBackCommand
        {
            get
            {
                if (signinPWBackCommand == null)
                {
                    signinPWBackCommand = new DelegateCommand(signinPWBack, canSigninPWBack);
                }
                return signinPWBackCommand;
            }
            set
            {
                signinPWBackCommand = value;
            }
        }

        public DelegateCommand CreateAccountCommand
        {
            get
            {
                if (createAccountCommand == null)
                {
                    createAccountCommand = new DelegateCommand(createAccount, canCreateAccount);
                }
                return createAccountCommand;
            }
            set
            {
                createAccountCommand = value;
            }
        }

        public DelegateCommand ForgetPWCommand
        {
            get
            {
                if (forgetPWCommand == null)
                {
                    forgetPWCommand = new DelegateCommand(forgetPW, canForgetPW);
                }
                return forgetPWCommand;
            }
            set
            {
                forgetPWCommand = value;
            }
        }
        #endregion command

        #region Command Delegate Method
        private void signinMailNext()
        {
            if (loginModel.isEmailAddress(Email))
            {
                prepare();
                Debug.WriteLine("mail valid");
                eventAggregator.GetEvent<SigninMailNextEvent>().Publish(0);
            }
            else
            {
                Debug.WriteLine("mail invalid");
                PromtInfo = emailInvalidMessage;
            }
        }
        private bool canSigninMailNext()
        {
            return !string.IsNullOrWhiteSpace(Email);
        }

        private void signinPWNext()
        {
            // TODO: show some animate which also help avoid user to click back
            // Password = "123456789";
            // Email = "sdf@sdf.com";
            loginModel.sendSigninRequest();
        }
        private bool canSigninPWNext()
        {
            return !string.IsNullOrWhiteSpace(Password);
            //return true;
        }

        private void signinPWBack()
        {
            // TODO: when user send signin request the button should be shutdowm?
            Password = null;
            PromtInfo = null;
            eventAggregator.GetEvent<SigninPWBackEvent>().Publish(Email);
        }
        private bool canSigninPWBack()
        {
            return true;
        }

        private void createAccount()
        {
            eventAggregator.GetEvent<CreateAccountEvent>().Publish(0);
        }
        private bool canCreateAccount()
        {
            return true;
        }

        private void forgetPW()
        {
            eventAggregator.GetEvent<ForgetPWEvent>().Publish(Email);
        }
        private bool canForgetPW()
        {
            return true;
        }
        #endregion Command Delegate Method

        #region event handler
        private void SigninResponseEventHandler(ProtoMessage message)
        {
            bool status = message.SigninResponse.Status;
            Debug.WriteLine(status);
            if (status)
            {
                // sign in successfully
                eventAggregator.GetEvent<SigninPWSigninEvent>().Publish(0);
            }
            else
            {
                // clear the pw and promt the user to try again or reset pw
                Password = null;
                PromtInfo = pwWrongMessage;
            }
        }
        #endregion event handler

    }
}