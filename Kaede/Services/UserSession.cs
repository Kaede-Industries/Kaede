﻿using Kaede.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Services
{
    public class UserSession
    {
        private readonly ReaderWriterLockSlim _lock = new();

        private UserDTO? _currentUser;
        public UserDTO? CurrentUser {
            get 
            {
                _lock.EnterReadLock();
                try
                {
                    return _currentUser;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            private set
            {
                _lock.EnterWriteLock();
                try
                {
                    _currentUser = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        public bool IsLoggedIn => CurrentUser != null;
        public void Login(UserDTO user)
        {
            if (IsLoggedIn)
                throw new InvalidOperationException("A logged in user is already present in the user service.");
            CurrentUser = user;
        }
        public void Logout() => CurrentUser = null;
    }
}
