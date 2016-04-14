import ko from 'knockout';
import template from 'text!./nav-bar.html';
import auth from 'app/auth';
import hasher from 'hasher';

class NavBarViewModel {
    constructor(params) {
        this.route = params.route;        
        this.authenticated = auth.authenticated; 
    }

    logout() {
        auth.logout(() => hasher.setHash(''));
    }
}

export default { viewModel: NavBarViewModel, template: template };