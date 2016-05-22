import ko from 'knockout';
import 'knockout-validation';
import invTemplate from 'text!./invitations.html';
import moment from 'moment';
import http from 'app/http';
import auth from 'app/auth';

class InvitationsViewModel {
    constructor() {

        // KoValidation config
        ko.validation.init({
            insertMessages: false
        });    

        // Modal 
        this.recipientEmail = ko.observable().extend({ email: true });
        this.validationModel = ko.validatedObservable({ email: this.recipientEmail });

        // Loading state
        this.loading = ko.observable(false);

        // Grid data
        this.gridData = ko.observableArray();

        if (auth.authenticated()) {
            this.reloadGrid();
        }
        else {
            auth.authenticated.subscribe((val) => { this.reloadGrid(); });
        }
    }
    
    addNewInvitation() {
        if (!this.validationModel.isValid()) {
            return false;
        }

        $.ajax({
            url: 'http://nbudgetcloudservice.cloudapp.net:8080/api/Invitations',
            method: 'POST',
            headers: { Authorization: 'Bearer ' + sessionStorage.getItem('token')},
            data: {
                RecipientEmail: this.recipientEmail()
            }})
            .done(data => this.reloadGrid()
            );
    }

    deleteInvitation(item) {
        $.ajax({
            url: 'http://nbudgetcloudservice.cloudapp.net:8080/api/Invitations/' + item.Id,
            method: 'PUT',
            headers: { Authorization: 'Bearer ' + sessionStorage.getItem('token')}}
            )
            .done(data => this.reloadGrid());
    }

    reloadGrid() {
        this.loading(true);

        $.ajax({
            url: 'http://nbudgetcloudservice.cloudapp.net:8080/api/Invitations',
            method: 'GET',
            headers: { Authorization: 'Bearer ' + sessionStorage.getItem('token')}
        }).done(invs => {
            this.fillWithData(invs);
            this.loading(false);
        });
    }

    fillWithData(data) {
        var invitations = data.map(item => {
            item.CreationDate = moment(item.CreationDate).format('YYYY-MM-DD');
            return item;
        });
        this.gridData(invitations);
    }
}

export default { viewModel: InvitationsViewModel, template: invTemplate };
