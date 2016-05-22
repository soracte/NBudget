import ko from 'knockout';
import 'knockout-validation';
import catTemplate from 'text!./categories.html';
import moment from 'moment';
import http from 'app/http';
import auth from 'app/auth';

class CategoriesViewModel {
    constructor() {

        // Modal 
        this.catName= ko.observable();

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
    
    addNewCategory() {
        var addedCategory = {
            Name: this.catName(),
        }

        http.post("http://nbudgetcloudservice.cloudapp.net:8080/api/Categories", addedCategory, result => this.reloadGrid());
    }

    reloadGrid() {
        this.loading(true);

        http.get("http://nbudgetcloudservice.cloudapp.net:8080/api/Categories", categories => {
            this.fillWithData(categories);
            this.loading(false);
        });
    }

    fillWithData(data) {
        var categories = data
        this.gridData(categories);
    }
}

export default { viewModel: CategoriesViewModel, template: catTemplate };
