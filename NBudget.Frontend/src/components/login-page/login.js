import ko from 'knockout';
import 'knockout-validation';
import loginTemplate from 'text!./login.html';
import hasher from 'hasher';
import auth from 'app/auth';

class LoginViewModel {
    constructor(params) {
        this.email = ko.observable();
        this.password = ko.observable();
        this.self = this;
    }

    login() {
        $.post("http://localhost:55880/Token",
                  { grant_type: 'password', username: this.email(), password: this.password() }
                  )
              .done(function (data) {
                  sessionStorage.setItem("token", data.access_token);
                  auth.authenticate(() => hasher.setHash(''));
              })
              .fail(function (data) {
                  var desc = JSON.parse(data.responseText).error_description;
                  console.log('Error: ' + desc);
              });

        return true;
    }
}

export default { viewModel: LoginViewModel, template: loginTemplate };