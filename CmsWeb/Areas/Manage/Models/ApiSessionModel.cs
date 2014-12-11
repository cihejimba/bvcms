﻿using System;
using CmsData;
using System.Linq;

namespace CmsWeb.Areas.Manage.Models
{
    internal static class ApiSessionModel
    {
        public static ApiSessionResult DetermineApiSessionStatus(Guid sessionToken, int? pin = null)
        {
            const int minutesSessionIsValid = 30;

            var session = DbUtil.Db.ApiSessions.SingleOrDefault(x => x.SessionToken == sessionToken);
            if (session == null)
                return new ApiSessionResult(null, ApiSessionStatus.SessionTokenNotFound);

            // if the user has stored a PIN before and it isn't populated or doesn't match, then we have an invalid PIN
            if (session.Pin.HasValue)
            {
                if (!pin.HasValue)
                    return new ApiSessionResult(session.User, ApiSessionStatus.PinInvalid);

                if (pin.Value != session.Pin)
                    return new ApiSessionResult(session.User, ApiSessionStatus.PinInvalid);
            }

            var isExpired = session.LastAccessedDate < DateTime.Now.Subtract(TimeSpan.FromMinutes(minutesSessionIsValid));

            if (isExpired)
            {
                return session.Pin.HasValue 
                    ? new ApiSessionResult(session.User, ApiSessionStatus.PinExpired) 
                    : new ApiSessionResult(session.User, ApiSessionStatus.SessionTokenExpired);
            }

            return new ApiSessionResult(session.User, ApiSessionStatus.Success);
        }

        public static void SaveApiSession(User user, int? pin)
        {
            var apiSession = user.ApiSessions.SingleOrDefault();
            if (apiSession != null)
            {
                apiSession.LastAccessedDate = DateTime.Now;
                apiSession.Pin = pin;
            }
            else
            {
                var now = DateTime.Now;
                apiSession = new ApiSession();
                apiSession.SessionToken = Guid.NewGuid();
                apiSession.LastAccessedDate = now;
                apiSession.CreatedDate = now;
                apiSession.Pin = pin;
                user.ApiSessions.Add(apiSession);
            }

            DbUtil.Db.SubmitChanges();
        }

        public static void ResetSessionExpiration(User user, int? pin/*, string sessionToken = null*/)
        {
            var apiSession = user.ApiSessions.SingleOrDefault();
            if (apiSession == null)
                return;

            apiSession.LastAccessedDate = DateTime.Now;
            DbUtil.Db.SubmitChanges();
        }
    }
}