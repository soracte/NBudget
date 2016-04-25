import ko from 'knockout';
import template from 'text!./nav-bar.html';
import app from 'app/app';
import auth from 'app/auth';
import hasher from 'hasher';

class NavBarViewModel {
    constructor(params) {
        this.route = params.route;        
        this.authenticated = auth.authenticated; 
        this.inviters = auth.inviters;
    }

    logout() {
        auth.logout(() => hasher.setHash(''));
    }

    changeToOwn() {
        auth.changeToOwn();
        return true;
    }

    changeOwner(item) {
        auth.changeOwner(item.Id);
        return true;
    }
}

export default { viewModel: NavBarViewModel, template: template };