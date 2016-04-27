import ko from 'knockout';
import 'knockout-validation';
import registerTemplate from 'text!./register.html';
import hasher from 'hasher';

class RegisterViewModel {

    constructor() {
        ko.validation.init({
            decorateInputElement: true,
            registerExtenders: true,
            messagesOnModified: true,
            errorElementClass: 'has-error',
            errorMessageClass: 'help-block',
        }); 

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
                params: this.password,
                message: 'Passwords do not match!'
            }
        });

        this.firstName = ko.observable().extend({ required: true });
        this.lastName = ko.observable().extend({ required: true });

        this.validationModel = ko.validatedObservable({email: this.email, password: this.password, confirmPassword: this.confirmPassword});
    }

    register() {
        if (!this.validationModel.isValid()) {
            return false;
        }

        $.ajax({
            url: "http://localhost:55880/api/Account/Register", 
            data:  {
                Email: this.email(),
                Password: this.password(),
                ConfirmPassword: this.confirmPassword(),
                FirstName: this.firstName(),
                LastName: this.lastName()
            },
            method: 'POST',
            headers: {Accept: 'application/json'}
        }
            )
        .done(function (data) {
            hasher.setHash('login/regSuccess');
        })
        .fail(function (data) {
            var desc = JSON.parse(data.responseText)['error_description'];
            console.log('Error: ' + desc);
        });
    }
};

export default { viewModel: RegisterViewModel, template: registerTemplate };
