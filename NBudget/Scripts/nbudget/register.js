var LoginModel = function () {
    var self = this;

    this.email = ko.observable().extend({ required: true, email: true });
    this.password = ko.observable().extend({ required: true });
    this.confirmPassword = ko.observable().extend({
        validation: {
            getValue: function (o) {
                return (typeof o === 'function' ? o() : o);
            },
            validator: function (val, otherField) {
                return val === this.getValue(otherField);
            },
            params: self.password,
            message: 'Passwords do not match!'
        }
    });

    this.register = function () {
        var errors = ko.validation.group(this);

        if (errors().length > 0) {
            return false;
        }

        $.post("/api/Account/Register",
            {
                Email: self.email(),
                Password: self.password(),
                ConfirmPassword: self.confirmPassword()
            }
            )
        .done(function (data) {
            sessionStorage.setItem("token", data['access_token'])
        })
        .fail(function (data) {
            var desc = JSON.parse(data.responseText)['error_description'];
            console.log('Error: ' + desc);
        })
    }
};

var loginModel = ko.validatedObservable(new LoginModel());

$(function () {
    ko.validation.init({
        decorateInputElement: true,
        registerExtenders: true,
        messagesOnModified: true,
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
    });    
    ko.applyBindings(loginModel);
});