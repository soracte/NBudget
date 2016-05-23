import ko from 'knockout';
import reportDetailsTemplate from 'text!./report-details.html';
import moment from 'moment';
import auth from 'app/auth';

class ReportDetailsViewModel {
    constructor(params) {

        this.creationDate = ko.observable();
        this.fromDate = ko.observable();
        this.toDate = ko.observable();
        this.topTransactions = ko.observableArray();
        this.categorySummary = ko.observableArray();

        // Loading state
        this.loading = ko.observable(false);

        this.loadData(params.reportId);
    }

    loadData(reportId) {
        this.loading(true);
        $.ajax({
            url: "http://nbudgetcloudservice.cloudapp.net:8080/api/ReportHeaders/" + auth.currentUserId() + "/" + reportId,
            headers: { Authorization: "Bearer " + sessionStorage.getItem("token")},
        })
        .done(data => {
            this.creationDate(this.getDate(data.Created));
            this.fromDate(this.getDate(data.FromDate));
            this.toDate(this.getDate(data.ToDate));
            this.topTransactions(data.TopTransactions);
            this.categorySummary(data.CategorySummary);
            this.loading(false);
        });
    }

    getDate(isoDate) {
        return moment(isoDate).format('YYYY-MM-DD');
    }
    
}

export default { viewModel: ReportDetailsViewModel, template: reportDetailsTemplate };
