import ko from 'knockout';
import 'knockout-validation';
import reportsTemplate from 'text!./reports.html';
import moment from 'moment';
import http from 'app/http';
import Pikaday from 'pikaday';

class ReportsViewModel {
    constructor() {

        // Modal 
        this.fromDate = ko.observable();
        this.toDate = ko.observable();

        // Loading state
        this.loading = ko.observable(false);

        // Grid data
        this.gridData = ko.observableArray();

        new Pikaday({
            field: document.getElementById('fromDate'),
            firstDay: 1,
            format: 'YYYY-MM-DD'
        })

        new Pikaday({
            field: document.getElementById('toDate'),
            firstDay: 1,
            format: 'YYYY-MM-DD'
        })



        this.reloadGrid();
    }
    
    addNewReport() {
        var addedReport = {
            FromDate: this.fromDate(),
            ToDate: this.toDate()
        }

        http.post("http://localhost:55880/api/ReportHeaders", addedReport, result => this.reloadGrid());
    }

    reloadGrid() {
        this.loading(true);

        http.get("http://localhost:55880/api/ReportHeaders", reports => {
            this.fillWithData(reports);
            this.loading(false);
        });
    }

    fillWithData(data) {
        var reports = data.map(item => {
            item.CreationDate = moment(item.CreationDate).format('YYYY-MM-DD');
            item.FromDate = moment(item.FromDate).format('YYYY-MM-DD');
            item.ToDate = moment(item.ToDate).format('YYYY-MM-DD');
            return item;
        });
        this.gridData(reports);
    }
}

export default { viewModel: ReportsViewModel, template: reportsTemplate };
