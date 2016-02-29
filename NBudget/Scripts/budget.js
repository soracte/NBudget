function Transaction(date, amount, reason, categoryId) {
    this.date = ko.observable(date);
    this.amount = ko.observable(amount);
    this.reason = ko.observable(reason);
    this.category = ko.observable(categoryId);
}

function Category(id, name) {
    this.id = ko.observable(id);
    this.name = ko.observable(name);
}

function mainVm() {
    var self = this;

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

    // Grid config
    this.gridOptions = {
        data: self.gridData,
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

    this.addNewTransaction = function (formElement) {
        var errors = ko.validation.group(self, { deep: true });

        if (errors().length > 0) {
            errors.showAllMessages(true);
            return false;
        }

        var addedTransaction = new Transaction(this.date(), this.amount(), this.reason(), this.category().id());

        $.ajax("/api/Transactions", {
            data: ko.toJSON(addedTransaction),
            type: "post", contentType: "application/json",
            success: function (result) {
                reloadGrid();
                resetVm();
            }
        });
    };

    this.fillWithData = function (data, categoryData) {
        var categories = ko.utils.arrayMap(categoryData, function (item) {
            return new Category(item.Id, item.Name);
        });

        this.categories(categories);

        var transactions = ko.utils.arrayMap(data, function (item) {
            var categoryName = ko.utils.arrayFirst(categoryData, function (cat) { return cat.Id == item.Category; }).Name;
            return new Transaction(moment(item.Date).format('YYYY-MM-DD'), item.Amount, item.Reason, categoryName);
        });

        this.gridData(transactions);

        
    };
};

var vm = new mainVm();

function reloadGrid() {
    var categories = [];

    loading = true;
    $.getJSON("/api/Categories", function (categoryData) {
        $.getJSON("/api/Transactions", function (data) {
            vm.fillWithData(data, categoryData);
            loading = false;
        });
    });
}

function resetVm() {
    vm.date(undefined);
    vm.amount(undefined);
    vm.reason(undefined);
    vm.category(undefined);
}

$(function () {
    reloadGrid();
    ko.applyBindings(vm);
});