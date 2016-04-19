import externalLoginTemplate from 'text!./external-login.html';
import hasher from 'hasher';
import auth from 'app/auth';
import deparam from 'jquery-deparam';

class ExternalLoginViewModel {
    constructor(params) {
        var token = this.getAccessToken(params);
        var tokenHeader = {'Authorization': 'Bearer ' + token};

        $.ajax({
            url: "http://localhost:55880/api/Account/UserInfo",
            headers: tokenHeader
        }).done(data => {
            if (data.HasRegistered === true) {
                sessionStorage.setItem('token', token);
                auth.authenticate(() => hasher.setHash(''));
            } else {
                $.ajax({
                    url: "http://localhost:55880/api/Account/RegisterExternal",
                    method: "POST",
                    headers: tokenHeader
                }).done(data => window.location = auth.facebookLoginUrl());
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