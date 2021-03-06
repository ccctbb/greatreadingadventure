﻿using System;
using System.Web;
using System.Web.UI;
using SRPApp.Classes;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.Tools;
using System.Web.UI.HtmlControls;

namespace GRA.SRP
{
    public partial class SRPMaster : BaseSRPMaster
    {
        public BaseSRPPage CurrentPage { get; set; }
        public string Unread { get; set; }
        public string SystemNameText {
            get {
                if (SRPPage != null)
                {
                    return SRPPage.GetResourceString("system-name");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string SloganText {
            get {
                if (SRPPage != null)
                {
                    return SRPPage.GetResourceString("slogan");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string DefaultMetaDescription {
            get {
                if (SRPPage != null)
                {
                    var result = SRPPage.GetResourceString("frontpage-description");
                    if (string.IsNullOrEmpty(result) || result.Equals("frontpage-description"))
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return result.Trim();
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string UpsellText {
            get {
                if (SRPPage != null)
                {
                    return SRPPage.GetResourceString("upsell");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string CopyrightStatementText {
            get {
                if (SRPPage != null)
                {
                    return SRPPage.GetResourceString("footer-copyright");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string RegisterPageActive {
            get {
                if (Request.Path.EndsWith("Register.aspx", StringComparison.OrdinalIgnoreCase)
                   || Request.Path.EndsWith("RegisterILS.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string LoginPageActive {
            get {
                if (Request.Path.EndsWith("Login.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string DashboardPageActive {
            get {
                if (Request.Path.EndsWith("Dashboard.aspx", StringComparison.OrdinalIgnoreCase)
                   || Request.Path.StartsWith("/Default.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string OffersPageActive {
            get {
                if (Request.Path.IndexOf("/Offers/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string MailSectionActive {
            get {
                if (Request.Path.IndexOf("/Mail/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string AdventuresSectionActive {
            get {
                if (Request.Path.IndexOf("/Adventures/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "active";
                }
                return string.Empty;
            }
        }
        public string ChallengesSectionActive {
            get {
                if (Request.Path.IndexOf("/Challenges/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "active";
                }
                return string.Empty;
            }
        }
        public string EventsSectionActive {
            get {
                if (Request.Path.IndexOf("/Events/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string BadgesSectionActive {
            get {
                if (Request.Path.IndexOf("/Badges/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string AccountSectionActive {
            get {
                if (Request.Path.IndexOf("/Account/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string EarnedBadges {
            get; set;
        }

        public bool ShowLoginPopup { get; set; }
        public string LoginPopupErrorMessage { get; set; }

        public string BasePath { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            object patronMessage = Session[SessionKey.PatronMessage];

            if (patronMessage != null)
            {
                object patronMessageLevel = Session[SessionKey.PatronMessageLevel];
                string alertLevel = "alert-success";
                if (patronMessageLevel != null)
                {
                    alertLevel = string.Format("alert-{0}", patronMessageLevel.ToString());
                    Session.Remove(SessionKey.PatronMessageLevel);
                }
                alertContainer.CssClass = string.Format("alert {0}",
                                                        alertLevel);
                alertGlyphicon.Visible = false;
                object patronMessageGlyph = Session[SessionKey.PatronMessageGlyphicon];
                if (patronMessageGlyph != null)
                {
                    alertGlyphicon.Visible = true;
                    alertGlyphicon.CssClass = string.Format("glyphicon glyphicon-{0} margin-halfem-right",
                                                            patronMessageGlyph);
                    Session.Remove(SessionKey.PatronMessageGlyphicon);
                }
                alertMessage.Text = patronMessage.ToString();
                alertContainer.Visible = true;
                Session.Remove(SessionKey.PatronMessage);
            }
            else
            {
                alertContainer.Visible = false;
            }

            if (!PatronTakingTest)
            {
                var earnedBadges = Session[SessionKey.EarnedBadges];
                if (earnedBadges != null)
                {
                    EarnedBadges = earnedBadges.ToString().Replace('|', ',');
                    new SessionTools(Session).ClearEarnedBadges();
                    if (EarnedBadges.Contains("?")) {
                        var currentPatron = Session[SessionKey.Patron] as Patron;
                        if(currentPatron != null)
                        {
                            this.Log().Warn("Invalid EarnedBadges value for {0} contains ?: {1}",
                                currentPatron.PID,
                                EarnedBadges);
                        }
                        else
                        {
                            this.Log().Warn("Invalid EarnedBadges value - contains ?: {0}", EarnedBadges);
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.PageLoad(sender, e);
            CurrentPage = (BaseSRPPage)Page;

            if (string.IsNullOrEmpty(Page.Title) && !string.IsNullOrEmpty(SystemNameText))
            {
                Page.Title = SystemNameText.Trim();
            }

            Control ctl = LoadControl("~/Controls/ProgramCSS.ascx");
            var plc = FindControl("ProgramCSS");
            plc.Controls.Add(ctl);

            if (CurrentPage.IsSecure && !CurrentPage.IsLoggedIn)
            {
                Response.Redirect("~/Logout.aspx");
            }

            if (string.IsNullOrEmpty(CurrentPage.MetaDescription))
            {
                CurrentPage.MetaDescription = DefaultMetaDescription;
            }

            Page.MetaDescription = CurrentPage.MetaDescription;

            var currentTenant = HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == ""
                ? -1
                : (int)HttpContext.Current.Session["TenantID"];


            var sessionTool = new SessionTools(Session);

            if (PatronTakingTest)
            {
                adventuresNav.Visible = false;
                challengesNav.Visible = false;
                offersNav.Visible = false;
                badgesNav.Visible = false;
                eventsNav.Visible = false;
                mailNav.Visible = false;
                homeNav.Visible = false;
                accountNav.Visible = false;
            }
            else
            {
                homeNav.Visible = true;
                homeNav.Attributes.Add("class", DashboardPageActive);
                mailNav.Visible = true;
                mailNav.Attributes.Add("class", MailSectionActive);
                accountNav.Visible = true;
                accountNav.Attributes.Add("class", AccountSectionActive);
                LoggedOutRegister.Attributes.Add("class", RegisterPageActive);
                LoggedOutLogin.Attributes.Add("class", LoginPageActive);

                var adventuresActive = sessionTool.GetCache(Cache, CacheKey.AdventuresActive) as bool?;
                if (adventuresActive == null)
                {
                    var programGames = DAL.ProgramGame.GetAll();
                    adventuresActive = programGames.Tables.Count > 0 && programGames.Tables[0].Rows.Count > 0;
                    sessionTool.SetCache(Cache, CacheKey.AdventuresActive, adventuresActive);
                }
                adventuresNav.Visible = adventuresActive == true;
                adventuresNav.Attributes.Add("class", AdventuresSectionActive);

                var challengesActive = sessionTool.GetCache(Cache, CacheKey.ChallengesActive) as bool?;
                if (challengesActive == null)
                {
                    var challenges = DAL.BookList.GetAll();
                    challengesActive = challenges.Tables.Count > 0 && challenges.Tables[0].Rows.Count > 0;
                    sessionTool.SetCache(Cache, CacheKey.ChallengesActive, challengesActive);
                }
                challengesNav.Visible = challengesActive == true;
                challengesAnonNav.Visible = challengesActive == true;
                challengesNav.Attributes.Add("class", ChallengesSectionActive);
                challengesAnonNav.Attributes.Add("class", ChallengesSectionActive);

                var offersActive = sessionTool.GetCache(Cache, CacheKey.OffersActive) as bool?;
                if (offersActive == null)
                {
                    var offers = DAL.Offer.GetAll();
                    offersActive = offers.Tables.Count > 0 && offers.Tables[0].Rows.Count > 0;
                    sessionTool.SetCache(Cache, CacheKey.OffersActive, offersActive);
                }
                offersNav.Visible = offersActive == true;
                offersNav.Attributes.Add("class", OffersPageActive);

                var badgesActive = sessionTool.GetCache(Cache, CacheKey.BadgesActive) as bool?;
                if (badgesActive == null)
                {
                    badgesActive = DAL.Badge.GetVisibleCount() > 0;
                    sessionTool.SetCache(Cache, CacheKey.BadgesActive, badgesActive);
                }
                badgesNav.Visible = badgesActive == true;
                badgesAnonNav.Visible = badgesActive == true;
                badgesNav.Attributes.Add("class", BadgesSectionActive);
                badgesAnonNav.Attributes.Add("class", BadgesSectionActive);

                var eventsActive = sessionTool.GetCache(Cache, CacheKey.EventsActive) as bool?;
                if (eventsActive == null)
                {
                    var events = DAL.Event.GetAll();
                    eventsActive = events.Tables.Count > 0 && events.Tables[0].Rows.Count > 0;
                    sessionTool.SetCache(Cache, CacheKey.EventsActive, eventsActive);
                }
                eventsNav.Visible = eventsActive == true;
                eventsAnonNav.Visible = eventsActive == true;
                eventsNav.Attributes.Add("class", EventsSectionActive);
                eventsAnonNav.Attributes.Add("class", EventsSectionActive);
            }

            if (!IsPostBack)
            {
                if (CurrentPage.IsLoggedIn)
                {
                    //f.Visible = ((Patron) Session["Patron"]).IsMasterAccount;
                    if (Session[SessionKey.IsMasterAccount] as bool? == true)
                    {
                        a.Title = "My Account & Family";
                    }
                    Unread = Notifications.GetAllUnreadToPatron(((Patron)Session["Patron"]).PID).Tables[0].Rows.Count.ToString();
                    if (!(Page is AddlSurvey || Page is Register || Page is Login || Page is Logout || Page is Recover))
                    {
                        if (Session["PreTestMandatory"] != null && (bool)Session["PreTestMandatory"])
                        {
                            TestingBL.PatronNeedsPreTest();
                        }
                    }
                }
                else
                {
                    loginPopupPanel.Visible = true;
                    if (Session[SessionKey.RequestedPath] != null)
                    {
                        ShowLoginPopup = true;
                        ViewState[SessionKey.RequestedPath] = Session[SessionKey.RequestedPath];
                        Session.Remove(SessionKey.RequestedPath);
                    }
                    if (Request.Cookies[CookieKey.Username] != null)
                    {
                        loginPopupUsername.Text = Request.Cookies[CookieKey.Username].Value;
                        loginPopupRememberMe.Checked = true;
                    }

                    string programId = Request["PID"];
                    if (string.IsNullOrEmpty(programId))
                    {
                        var sessProgId = Session["ProgramID"];
                        if (sessProgId != null)
                        {
                            programId = sessProgId.ToString();
                        }
                    }
                    if (string.IsNullOrEmpty(programId))
                    {
                        programId = Programs.GetDefaultProgramID().ToString();
                    }
                    var program = DAL.Programs.FetchObject(int.Parse(programId));

                    if (!program.IsRegistrationOpen)
                    {
                        LoggedOutRegister.Visible = false;
                    }
                    if (!program.IsOpen && !program.IsRegistrationOpen)
                    {
                        LoggedOutLogin.Visible = false;
                    }
                }
            }
        }

        protected void loginPopupClick(object sender, EventArgs e)
        {
            if (!(string.IsNullOrEmpty(loginPopupUsername.Text.Trim())
               || string.IsNullOrEmpty(loginPopupPassword.Text.Trim())))
            {
                var patron = new Patron();
                if (Patron.Login(loginPopupUsername.Text.Trim(), loginPopupPassword.Text))
                {
                    var bp = Patron.GetObjectByUsername(loginPopupUsername.Text.Trim());

                    var pgm = DAL.Programs.FetchObject(bp.ProgID);
                    if (pgm == null)
                    {
                        int schoolGrade;
                        int.TryParse(bp.SchoolGrade, out schoolGrade);
                        var progID = Programs.GetDefaultProgramForAgeAndGrade(bp.Age, schoolGrade);
                        bp.ProgID = progID;
                        bp.Update();
                    }


                    /* recalulate goal cache to accomdate changes in program length and point multipliers */
                    ProgramGamePointConversion pgc = null;

                    foreach (ActivityType activityTypeValue in Enum.GetValues(typeof(ActivityType)))
                    {
                        int activityTypeId = (int)activityTypeValue;
                        var temp = ProgramGamePointConversion.FetchObjectByActivityId(pgm.PID,
                                                                                 activityTypeId);

                        if (temp != null && temp.PointCount > 0)
                        {
                            pgc = temp;
                        }
                    }

                    if (pgc != null)
                    {
                        bp.RecalculateGoalCache(pgm, pgc);
                        bp.Update();
                    }


                    new SessionTools(Session).EstablishPatron(bp);

                    TestingBL.CheckPatronNeedsPreTest();
                    TestingBL.CheckPatronNeedsPostTest();

                    if (loginPopupRememberMe.Checked)
                    {
                        var loginUsernameCookie = new HttpCookie(CookieKey.Username);
                        loginUsernameCookie.Expires = DateTime.Now.AddDays(14);
                        loginUsernameCookie.Value = loginPopupUsername.Text.Trim();
                        Response.SetCookie(loginUsernameCookie);
                    }
                    else
                    {
                        if (Request.Cookies[CookieKey.Username] != null)
                        {
                            Response.Cookies[CookieKey.Username].Expires = DateTime.Now.AddDays(-1);
                        }
                    }

                    if (Session[SessionKey.RequestedPath] != null)
                    {
                        string requestedPath = Session[SessionKey.RequestedPath].ToString();
                        Session.Remove(SessionKey.RequestedPath);
                        Response.Redirect(requestedPath);
                    }
                    else if (ViewState[SessionKey.RequestedPath] != null)
                    {
                        string requestedPath = ViewState[SessionKey.RequestedPath].ToString();
                        Response.Redirect(requestedPath);
                    }
                    else
                    {
                        Response.Redirect("~");
                    }
                }
                else
                {
                    LoginPopupErrorMessage = "Invalid username or password.";
                    new SessionTools(Session).ClearPatron();
                }
            }

        }

    }
}

