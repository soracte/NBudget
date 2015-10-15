function mainVm() {
    var self = this;
    this.myData = ko.observableArray([]);

    this.gridOptions = {
        data: self.myData,
        columnDefs: [{ field: 'Date', displayName: 'Dátum', sortable: true, direction: 'asc' },
                     { field: 'Amount', displayName: 'Összeg', sortable: false },
                     { field: 'Reason', displayName: 'Megnevezés', sortable: false }],
        sortInfo: {
            column: 'Date', //any column name
            direction: 'asc' // values can be 'asc' or 'desc'
        }
    };

    this.addNewTransaction = function (formElement) {
        var dateText = $(formElement).find("#datepicker").val();
        var amountText = $(formElement).find("#amount").val();
        var reasonText = $(formElement).find("#reason").val();

        var added = { date: dateText, amount: amountText, reason: reasonText };


        $.ajax("/api/Transactions", {
            data: ko.toJSON(added),
            type: "post", contentType: "application/json",
            success: function (result) { self.myData.push({ Date: dateText, Amount: amountText, Reason: reasonText }); }
        });
    };

    this.initializeWithData = function (data) {
        this.myData(data);
    }
};

$(function () {
    $.getJSON("/api/Transactions", function (data) {
        var vm = new mainVm();
        vm.initializeWithData(data);

        // TODO probably call this after each grid modification and sort on server side
        

        ko.applyBindings(vm);
    });
});