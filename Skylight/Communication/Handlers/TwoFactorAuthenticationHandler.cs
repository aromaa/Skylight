using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Handlers
{
    public class TwoFactorAuthenticationHandler : MessageHandler
    {
        public static readonly Guid DefaultIdentifier = Guid.Parse("a3fd0572-45c4-4963-ab2c-3994507242af");
        private readonly Guid CurrentIdentifier = TwoFactorAuthenticationHandler.DefaultIdentifier;

        private int Tries;
        private readonly int MaxTries = 3;
        public readonly Stopwatch Started;
        private Queue<ClientMessage> MessagesPending;

        public TwoFactorAuthenticationHandler(int failures)
        {
            this.Tries = this.MaxTries - failures;
            this.Started = Stopwatch.StartNew();
            this.MessagesPending = new Queue<ClientMessage>();
        }

        public override Guid Identifier()
        {
            return this.CurrentIdentifier;
        }

        public override bool HandleMessage(GameClient session, ClientMessage message)
        {
            if (session.Revision == Revision.RELEASE63_35255_34886_201108111108)
            {
                if (message.GetID() == r63aIncoming.ChangeUsername || message.GetID() == r63aIncoming.CheckUsername)
                {
                    string sCode = message.PopFixedString();
                    if (sCode.Length == 6 && int.TryParse(sCode, out int code))
                    {
                        if (session.LoginUsingTwoFactorAuthenication(code))
                        {
                            session.LastPong.Restart(); //lets be sure we dont get dcd now as all packets are ignored x)
                            session.RemoveMessageHandler(this.Identifier());

                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.UsernameChanged);
                            message_.AppendInt32(0); //result
                            session.SendMessage(message_);
                        }
                        else
                        {
                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                                dbClient.AddParamWithValue("userIp", session.GetIP());
                                dbClient.AddParamWithValue("timestamp", TimeUtilies.GetUnixTimestamp());
                                dbClient.ExecuteQuery("INSERT INTO user_2fa_failures(user_id, ip, timestamp) VALUES(@userId, @userIp, @timestamp)");
                            }

                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.ValidUsername);
                            message_.AppendInt32(4); //result
                            message_.AppendString(sCode);
                            message_.AppendInt32(0); //suggested names
                            session.SendMessage(message_);

                            this.Tries--;
                            if (this.Tries <= 0)
                            {
                                session.SendNotif("You have failed the two factory authenication! You will be disconnected!");
                                session.Stop("Two factory authenication failed");
                            }
                            else
                            {
                                session.SendNotif("You have entered the wrong code! You have " + (this.Tries) + " more tries left! IF THE CORRECT CODE SOMEHOW DOSENT WORK, PLEASE WAIT FOR NEW TO GENERATE!", 2);
                            }
                        }
                    }
                    else
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(4); //result
                        message_.AppendString(sCode);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);

                        session.SendNotif("You must enter the two factory authenication code! Its lenght of six and only numbers", 2);
                    }
                }
                else
                {
                    this.MessagesPending.Enqueue(message);
                }
            }
            else if (session.Revision == Revision.PRODUCTION_201601012205_226667486)
            {
                if (message.GetID() == r63cIncoming.VerifyMobilePhoneCode)
                {
                    string sCode = message.PopFixedString();
                    if (sCode.Length == 6 && int.TryParse(sCode, out int code))
                    {
                        if (session.LoginUsingTwoFactorAuthenication(code))
                        {
                            session.LastPong.Restart();
                            session.RemoveMessageHandler(this.Identifier());

                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.VerifyMobilePhoneDone);
                            message_.AppendInt32(3);
                            message_.AppendInt32(0);
                            session.SendMessage(message_);

                            while (this.MessagesPending.Count > 0)
                            {
                                session.HandlePacket(this.MessagesPending.Dequeue());
                            }
                        }
                        else
                        {
                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                                dbClient.AddParamWithValue("userIp", session.GetIP());
                                dbClient.AddParamWithValue("timestamp", TimeUtilies.GetUnixTimestamp());
                                dbClient.ExecuteQuery("INSERT INTO user_2fa_failures(user_id, ip, timestamp) VALUES(@userId, @userIp, @timestamp)");
                            }

                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.VerifyMobilePhoneCodeWindow);
                            message_.AppendInt32(3);
                            message_.AppendInt32(0);
                            session.SendMessage(message_);

                            this.Tries--;
                            if (this.Tries <= 0)
                            {
                                session.SendNotif("You have failed the two factory authenication! You will be disconnected!");
                                session.Stop("Two factory authenication failed");
                            }
                            else
                            {
                                session.SendNotif("You have entered the wrong code! You have " + (this.Tries) + " more tries left! IF THE CORRECT CODE SOMEHOW DOSENT WORK, PLEASE WAIT FOR NEW TO GENERATE!", 2);
                            }
                        }
                    }
                    else
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.VerifyMobilePhoneCodeWindow);
                        message_.AppendInt32(3);
                        message_.AppendInt32(0);
                        session.SendMessage(message_);

                        session.SendNotif("You must enter the two factory authenication code! Its lenght of six and only numbers", 2);
                    }
                }
                else
                {
                    this.MessagesPending.Enqueue(message);
                }
            }
            
            return false; //NONE CAN BYPASS, EVEN PING IGNORED
        }

        public override bool Equals(object obj)
        {
            if (obj is TwoFactorAuthenticationHandler)
            {
                return ((TwoFactorAuthenticationHandler)obj).Identifier() == this.Identifier();
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.CurrentIdentifier.GetHashCode();
        }
    }
}
