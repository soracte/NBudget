function Transaction(date, amount, reason) {
    this.date = ko.observable(date);
    this.amount = ko.observable(amount);
    this.reason = ko.observable(reason);
}

function mainVm() {
    var self = this;

    ko.validation.init({
        decorateInputElement: true,
        registerExtenders: true,
        messagesOnModified: true,
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
    });

    this.date = ko.observable().extend({ required: true, date: true });
    this.amount = ko.observable().extend({ required: true, number: true });
    this.reason = ko.observable().extend({ required: true, maxLength: 20 });

    this.gridData = ko.observableArray([]);

    this.gridOptions = {
        data: self.gridData,
        columnDefs: [{ field: 'date', displayName: 'Dátum', sortable: true, direction: 'asc' },
                     { field: 'amount', displayName: 'Összeg', sortable: false },
                     { field: 'reason', displayName: 'Megnevezés', sortable: false }],
        sortInfo: ko.observable({
            column: { field: "date" },
            direction: "asc"
        })
    };

    this.addNewTransaction = function (formElement) {
        var errors = ko.validation.group([this.date, this.amount, this.reason]);

        if (errors) {
            return;
        }

        var addedTransaction = new Transaction(this.date(), this.amount(), this.reason());

        $.ajax("/api/Transactions", {
            data: ko.toJSON(addedTransaction    ),
            type: "post", contentType: "application/json",
            success: function (result) { self.gridData.push(addedTransaction); }
        });
    };

    this.initializeWithData = function (data) {
        var transactions = ko.utils.arrayMap(data, function (item) {
            return new Transaction(moment(item.Date).format('YYYY-MM-DD'), item.Amount, item.Reason);
        });

        this.gridData(transactions);
    };
};

$(function () {
    $.getJSON("/api/Transactions", function (data) {
        var vm = new mainVm();    
        vm.initializeWithData(data);

        ko.applyBindings(vm);

        $("#loadingLabel").hide();
    });
});