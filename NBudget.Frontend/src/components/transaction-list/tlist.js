import ko from 'knockout';
import 'knockout-validation';
import tlistTemplate from 'text!./tlist.html';
import moment from 'moment';
import Pikaday from 'pikaday';
import http from 'app/http';
import auth from 'app/auth';

class Transaction {
    constructor(date, amount, reason, categoryId) {
        this.date = ko.observable(date);
        this.amount = ko.observable(amount);
        this.reason = ko.observable(reason);
        this.category = ko.observable(categoryId);
    }
}

class Category {
    constructor(id, name) {
        this.id = ko.observable(id);
        this.name = ko.observable(name);
    }
}

class TransactionListViewModel {
    constructor(route) {
        this.message = ko.observable('Welcome to NBudget!');
               // KO config
        ko.validation.init({
            decorateInputElement: true,
            registerExtenders: true,
            messagesOnModified: true,
            errorElementClass: 'has-error',
            errorMessageClass: 'help-block',
        });    

        // Input view model
        this.date = ko.observable().extend({ required: true, date: true });
        this.amount = ko.observable().extend({ required: true, number: true });
        this.reason = ko.observable().extend({ required: true, maxLength: 20 });
        this.category = ko.observable().extend({ required: true });
        this.mode = ko.observable('income');

        // Loading state
        this.loading = ko.observable(false);

        // Grid data
        this.gridData = ko.observableArray();
        this.categories = ko.observableArray();

        // Modal
        this.isModelValid = ko.computed(() => ko.validation.group(this, { deep: true })().length === 0);

        // Filter
        this.checkedCategories = ko.observableArray();

        // Validation
        this.errors= ko.validation.group(this, { deep: true });
        var picker = new Pikaday({
            field: document.getElementById("datepicker"),
            firstDay: 1,
            format: 'YYYY-MM-DD'
        });

        if (auth.authenticated()) {
            this.reloadGrid();
        }
        else {
            auth.authenticated.subscribe((val) => { this.reloadGrid(); });
        }


    }

    toIncome() {
        this.mode('income');
    }

    toExpense() {
        this.mode('expense');
    }
    
    addNewTransaction(formElement) {
        if (this.errors().length > 0) {
            this.errors.showAllMessages(true);
            return false;
        }

        var amount = this.amount();
        if (this.mode() == 'expense') {
            amount = -amount;
        }
        var addedTransaction = new Transaction(this.date(), amount, this.reason(), this.category().id());

        http.post("http://nbudgetcloudservice.cloudapp.net:8080/api/Transactions/", addedTransaction, result => {
            this.reloadGrid();
            this.resetVm();
            $('#newTransactionModal').modal('hide');
        });
    }

    reloadGrid() {
        var categories = [];
        this.loading(true);

        http.get("http://nbudgetcloudservice.cloudapp.net:8080/api/Categories", cats => {
            var path = "http://nbudgetcloudservice.cloudapp.net:8080/api/Transactions" + ('?' + $.param({ cats: this.checkedCategories() }, true) || '');
            http.get(path, data => {
                this.fillWithData(data, cats);
                this.loading(false);
            })
        })
    }

    filterByCategories() {
        this.reloadGrid();
        return true;
    }
    
    fillWithData(data, categoryData) {
        var categories = categoryData.map(item => new Category(item.Id, item.Name));
        this.categories(categories);

        var transactions = data.map(item => {
            var categoryName = ko.utils.arrayFirst(categoryData, cat => cat.Id == item.Category).Name;
            return new Transaction(moment(item.Date).format('YYYY-MM-DD'), item.Amount, item.Reason, categoryName);
        });

        this.gridData(transactions);
    }

    resetVm() {
        this.date(undefined);
        this.amount(undefined);
        this.reason(undefined);
        this.category(undefined);
        this.errors.showAllMessages(false);
    }
}

export default { viewModel: TransactionListViewModel, template: tlistTemplate };
