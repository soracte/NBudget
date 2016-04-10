import ko from 'knockout';

class Auth {
    constructor() {
        var token = sessionStorage.getItem('token');
        this.authenticated = ko.observable(token !== null && token !== undefined);
    }
}

var auth = new Auth();

export default auth;