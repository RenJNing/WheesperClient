﻿using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Wheesper.Login.ViewModel;
using Wheesper.Login.Model;
using Wheesper.Login.View;

namespace Wheesper.Login
{
    public class LoginModule : IModule
    {
        #region private member
        private IUnityContainer container = null;
        private LoginController controller = null;
        #endregion private member

        #region Constructor
        public LoginModule(IUnityContainer container)
        {
            this.container = container;
            RegisterInstance();
        }
        #endregion Constructor

        # region Private Method
        private void RegisterInstance()
        {
            LoginModel loginModel = container.Resolve<LoginModel>();
            container.RegisterInstance(typeof(LoginModel), loginModel);

            SigninViewModel signinViewModel = container.Resolve<SigninViewModel>();
            container.RegisterInstance(typeof(SigninViewModel), signinViewModel);
            SignupViewModel signupViewModel = container.Resolve<SignupViewModel>();
            container.RegisterInstance(typeof(SignupViewModel), signupViewModel);
            PWModifyViewModel pwModifyViewModel = container.Resolve<PWModifyViewModel>(); 
            container.RegisterInstance(typeof(PWModifyViewModel), pwModifyViewModel);


            SigninMailView signinMailView = container.Resolve<SigninMailView>();
            container.RegisterInstance(typeof(SigninMailView), signinMailView);
            SigninPWView signinPWView = container.Resolve<SigninPWView>();
            container.RegisterInstance(typeof(SigninPWView), signinPWView);
        }
        # endregion Private Method

        #region IModule Methods
        public void Initialize()
        {
            controller = container.Resolve<LoginController>();
        }
        #endregion IModele Methods
    }
}
