function Transaction(date, amount, reason) {
    this.date = ko.observable(date);
    this.amount = ko.observable(amount);
    this.reason = ko.observable(reason);
}

function mainVm() {
    var self = this;
    this.myData = ko.observableArray([]);

    this.gridOptions = {
        data: self.myData,
        columnDefs: [{ field: 'date', displayName: 'Dátum', sortable: true, direction: 'asc' },
                     { field: 'amount', displayName: 'Összeg', sortable: false },
                     { field: 'reason', displayName: 'Megnevezés', sortable: false }],
        sortInfo: ko.observable({
            column: { field: "date" },
            direction: "asc"
        })
    };

    this.addNewTransaction = function (formElement) {
        var dateText = $(formElement).find("#datepicker").val();
        var amountText = $(formElement).find("#amount").val();
        var reasonText = $(formElement).find("#reason").val();

        var added = new Transaction(dateText, amountText, reasonText);

        $.ajax("/api/Transactions", {
            data: ko.toJSON(added),
            type: "post", contentType: "application/json",
            success: function (result) { self.myData.push({ date: dateText, amount: amountText, reason: reasonText }); }
        });
    };

    this.initializeWithData = function (data) {
        var transactions = ko.utils.arrayMap(data, function (item) {
            return new Transaction(moment(item.Date).format('YYYY-MM-DD'), item.Amount, item.Reason);
        });

        this.myData(transactions);
    }
};

$(function () {
    ko.validation.init();
    $.getJSON("/api/Transactions", function (data) {
        var vm = new mainVm();    
        vm.initializeWithData(data);

        ko.applyBindings(vm);
    });
});