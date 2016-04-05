import ko from 'knockout';
import 'knockout-validation';
import tlistTemplate from 'text!./tlist.html';

class Transaction {
    constructor(date, amount, reason, categoryId) {
        this.date = ko.observable(date);
        this.amount = ko.observable(amount);
        this.reason = ko.observable(reason);
        this.category = ko.observable(categoryId);
    }
}

class Category {
    category(id, name) {
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

        // Loading state
        this.loading = false;

        // Grid data
        this.gridData = ko.observableArray();
        this.categories = ko.observableArray();


        var self = this;
        // Modal
        this.isModelValid = ko.computed(() => ko.validation.group(this, { deep: true })().length == 0);

        // Filter
        this.checkedCategories = ko.observableArray();

        // Grid config
        this.gridOptions = {
            data: this.gridData,
            columnDefs: [{ field: 'date', displayName: 'Dátum', sortable: true, direction: 'asc' },
                         { field: 'amount', displayName: 'Összeg', sortable: false },
                         { field: 'reason', displayName: 'Megnevezés', sortable: false },
                         { field: 'category', displayName: 'Kategória', sortable: false }],
            sortInfo: ko.observable({
                column: { field: "date" },
                direction: "asc"
            }),
            displaySelectionCheckbox: false,
        };

        this.reloadGrid();
    }
    
    doSomething() {
        this.message('You invoked doSomething() on the viewmodel.');
    }

    addNewTransaction(formElement) {
        var errors = ko.validation.group(this, { deep: true });

        if (errors().length > 0) {
            errors.showAllMessages(true);
            return false;
        }

        var addedTransaction = new Transaction(this.date(), this.amount(), this.reason(), this.category().id());

        $.ajax("/api/Transactions", {
            data: ko.toJSON(addedTransaction),
            type: "post",
            contentType: "application/json",
            success: result => {
                reloadGrid();
                resetVm();
                $('#newTransactionModal').modal('hide');
            }
        });
    };

    reloadGrid() {
        var categories = [];

        this.loading = true;
        $.getJSON("/api/Categories", categoryData => {
            var path = "/api/Transactions" + ('?' + $.param({ catid: vm.checkedCategories() }, true) || '');
            $.getJSON(path, data => {
                vm.fillWithData(data, categoryData);
                this.loading = false;
            });
        });
    }

    filterByCategories() {
        reloadGrid();
        return true;
    }
    
    fillWithData(data, categoryData) {
        var categories = ko.utils.arrayMap(categoryData, item => {
            return new Category(item.Id, item.Name);
        });

        this.categories(categories);

        var transactions = ko.utils.arrayMap(data, item => {
            var categoryName = ko.utils.arrayFirst(categoryData, cat => cat.Id == item.Category).Name;
            return new Transaction(moment(item.Date).format('YYYY-MM-DD'), item.Amount, item.Reason, categoryName);
        });

        this.gridData(transactions);
    };
}

export default { viewModel: TransactionListViewModel, template: tlistTemplate };