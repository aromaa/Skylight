using Google.Authenticator;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class TwoFactoryAuthenicationComand : Command
    {
        public override string CommandInfo()
        {
            return ":2fa - Manage your two factory authenication settings, DONT USE IF YOU DONT KNOW WHAT IT IS";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            try
            {
                if (args.Length >= 2)
                {
                    if (args[1] == "enable")
                    {
                        if (!session.GetHabbo().IsTwoFactorAuthenticationEnabled())
                        {
                            if (string.IsNullOrEmpty(session.GetHabbo().TempTwoFactoryAuthenicationSecretCode))
                            {
                                session.GetHabbo().TempTwoFactoryAuthenicationSecretCode = TextUtilies.GenerateRandomString(12);
                            }

                            SetupCode code = new TwoFactorAuthenticator().GenerateSetupCode("Skylight", session.GetHabbo().Username, session.GetHabbo().TempTwoFactoryAuthenicationSecretCode, 300, 300);
                            session.SendNotifWithLink("You have requested to enable two factory authenication!\nManual code:" + code.ManualEntryKey + "\nQR Code: Open the link", code.QrCodeSetupImageUrl);
                        }
                        else
                        {
                            session.SendNotif("Two factory authenication is already enabled on yoru account!");
                        }
                    }
                    else if (args[1] == "confirm")
                    {
                        if (!session.GetHabbo().IsTwoFactorAuthenticationEnabled())
                        {
                            if (!string.IsNullOrEmpty(session.GetHabbo().TempTwoFactoryAuthenicationSecretCode))
                            {
                                if (args.Length >= 3)
                                {
                                    int code;
                                    if (args[2].Length == 6 && int.TryParse(args[2], out code))
                                    {
                                        if (new TwoFactorAuthenticator().ValidateTwoFactorPIN(session.GetHabbo().TempTwoFactoryAuthenicationSecretCode, code.ToString()))
                                        {
                                            session.GetHabbo().TwoFactoryAuthenicationSecretCode = session.GetHabbo().TempTwoFactoryAuthenicationSecretCode;
                                            session.GetHabbo().TempTwoFactoryAuthenicationSecretCode = "";

                                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                            {
                                                dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                                                dbClient.AddParamWithValue("secret", session.GetHabbo().TwoFactoryAuthenicationSecretCode);
                                                dbClient.ExecuteQuery("UPDATE users SET two_factory_authenication_secret_code = @secret WHERE id = @userId LIMIT 1");
                                            }

                                            session.SendNotif("Two factory authenicator enabled!");
                                        }
                                        else
                                        {
                                            session.SendNotif("Wrong code!");
                                        }
                                    }
                                    else
                                    {
                                        session.SendNotif("Code must be lenght of six and contains only numbers");
                                    }
                                }
                                else
                                {
                                    session.SendNotif("Please enter your current code to enable two factory authenication!");
                                }
                            }
                            else
                            {
                                session.SendNotif("You haven't requested to enable two factory authenication!");
                            }
                        }
                        else
                        {
                            session.SendNotif("Two factory authenication is already enabled on yoru account!");
                        }
                    }
                    else if (args[1] == "disable")
                    {
                        if (session.GetHabbo().IsTwoFactorAuthenticationEnabled())
                        {
                            if (args.Length >= 3)
                            {
                                int code;
                                if (args[2].Length == 6 && int.TryParse(args[2], out code))
                                {
                                    if (session.CheckTwoFactorAuthenicationCode(code))
                                    {
                                        session.GetHabbo().TwoFactoryAuthenicationSecretCode = "";

                                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                        {
                                            dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                                            dbClient.ExecuteQuery("UPDATE users SET two_factory_authenication_secret_code = '' WHERE id = @userId LIMIT 1");
                                        }

                                        session.SendNotif("Two factory authenicator disabled!");
                                    }
                                    else
                                    {
                                        session.SendNotif("Wrong code!");
                                    }
                                }
                                else
                                {
                                    session.SendNotif("Code must be lenght of six and contains only numbers");
                                }
                            }
                            else
                            {
                                session.SendNotif("Please enter your current code to disable two factory authenication!");
                            }
                        }
                        else
                        {
                            session.SendNotif("Two factory authenication isin't enabled on your account!");
                        }
                    }
                }
                else
                {
                    if (session.GetHabbo().IsTwoFactorAuthenticationEnabled())
                    {
                        session.SendNotif("Two factory authenication: Enabled");
                    }
                    else
                    {
                        session.SendNotif("Two factory authenication: Disabled");
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.LogCommandException(ex.ToString());
                session.SendNotif("Ooops, error!");
            }

            return true;
        }
    }
}
