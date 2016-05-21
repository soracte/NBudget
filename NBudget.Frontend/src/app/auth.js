import ko from 'knockout';

class Auth {
    constructor() {
        var token = sessionStorage.getItem('token');
        this.authenticated = ko.observable(false);
        this.principal = ko.observable();
        this.facebookLoginUrl = ko.observable();
        this.googleLoginUrl = ko.observable();
        this.inviters = ko.observableArray();
        this.currentUserId = ko.observable();
        this.currentUserFirstName = ko.observable();

        this.loadExternalAuthenticationOptions();
        this.authenticate();
    }

    authenticate(cb) {
        if (!this.hasToken()) {
            return;
        }

        $.ajax({
            url: "http://localhost:55880/api/Account/UserInfo",
            headers: { 'Authorization' : 'Bearer ' + sessionStorage.getItem('token')}
        })
        .done(data => {
            this.principal({ "id": data.Id, "fname": data.FirstName, "lname": data.LastName, "email": data.Email });
            this.inviters(data.Inviters);
            this.changeToOwn();            
            this.authenticated(true);
            (cb || $.noop)();
        });
    }

    logout(cb) {
        if (!this.hasToken()) {
            return;
        }

        this.authenticated(false);
        this.principal({});
        sessionStorage.removeItem('token');

        (cb || $.noop)();
    }

    hasToken() {
        var token = sessionStorage.getItem('token');
        return token !== null && token !== undefined;
    }

    loadExternalAuthenticationOptions() {
        $.get("http://localhost:55880/api/Account/ExternalLogins?returnUrl=http://localhost:8080&generateState=true")
        .done(data => {
            var facebookLogin = data.find(login => login.Name === 'Facebook')
            if (facebookLogin) {
                this.facebookLoginUrl('http://localhost:55880' + facebookLogin.Url);
            }
            var googleLogin = data.find(login => login.Name === 'Google')
            if (googleLogin) {
                this.googleLoginUrl('http://localhost:55880' + googleLogin.Url);
            }
        })
    }

    changeToOwn() {
        this.currentUserId(this.principal().id);
        this.currentUserFirstName(this.principal().fname);
    }

    changeOwner(id) {
        this.currentUserId(id);
        this.currentUserFirstName(this.inviters().find(inv => inv.Id == id).FirstName);
    }
}

var auth = new Auth();

export default auth;