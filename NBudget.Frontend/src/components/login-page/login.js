import ko from 'knockout';
import 'knockout-validation';
import loginTemplate from 'text!./login.html';

class LoginViewModel {
    constructor(route) {
        this.email= ko.observable();
        this.password = ko.observable();

        this.hasToken = ko.computed(function () {
            return sessionStorage.getItem('token');
        });
    }

    login() {
        $.post("/Token",
                  { grant_type: 'password', username: self.email(), password: self.password() }
                  )
              .done(function (data) {
                  sessionStorage.setItem("token", data['access_token'])
              })
              .fail(function (data) {
                  var desc = JSON.parse(data.responseText)['error_description'];
                  console.log('Error: ' + desc);
              })

        return true;
    }
}

export default { viewModel: LoginViewModel, template: loginTemplate};