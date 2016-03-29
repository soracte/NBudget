var LoginModel = function () {
    var self = this;

    this.email= ko.observable();
    this.password = ko.observable();

    this.login = function () {
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
    };
}

$(function () {
    vm = new LoginModel();
    ko.applyBindings(vm);
});