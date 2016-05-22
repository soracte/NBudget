import externalLoginTemplate from 'text!./external-login.html';
import hasher from 'hasher';
import auth from 'app/auth';
import deparam from 'jquery-deparam';

class ExternalLoginViewModel {
    constructor(params) {
        var token = this.getAccessToken(params);
        var tokenHeader = {'Authorization': 'Bearer ' + token};

        var loginProviderUrls = {
            'Facebook': auth.facebookLoginUrl,
            'Google': auth.googleLoginUrl
        }

        $.ajax({
            url: "http://nbudgetcloudservice.cloudapp.net:8080/api/Account/UserInfo",
            headers: tokenHeader
        }).done(data => {
            var loginProviderName = data.LoginProvider;
            if (data.HasRegistered === true) {
                sessionStorage.setItem('token', token);
                auth.authenticate(() => hasher.setHash(''));
            } else {
                if (data.HasLocalEmail === true) {
                    $.ajax({
                        url: "http://nbudgetcloudservice.cloudapp.net:8080/api/Account/AddExternalLogin",
                        method: "POST",
                        headers: tokenHeader
                    }).done(data => window.location = loginProviderUrls[loginProviderName]());
                }
                else {
                    $.ajax({
                        url: "http://nbudgetcloudservice.cloudapp.net:8080/api/Account/RegisterExternal",
                        method: "POST",
                        headers: tokenHeader
                    }).done(data => window.location = loginProviderUrls[loginProviderName]());
                }
            } 
        });
    }

    getAccessToken(params) {
        var fragment = deparam(params.token);
        for (var prop in fragment)  {
            if(fragment[prop] === '') {
                return prop;
            }
        }
    }

}

export default { viewModel: ExternalLoginViewModel, template: externalLoginTemplate };