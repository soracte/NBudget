var LoginModel = function () {
    var self = this;

    this.email= ko.observable();
    this.password = ko.observable();

    this.hasToken = ko.computed(function () {
        return sessionStorage.getItem('token');
    })

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

        return true;
    };
}

$(function () {
    vm = new LoginModel();
    ko.applyBindings(vm);
});