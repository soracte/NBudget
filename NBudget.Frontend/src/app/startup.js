import 'jquery';
import 'bootstrap';
import ko from 'knockout';
import 'knockout-projections';
import app from './app';

// Components can be packaged as AMD modules, such as the following:
ko.components.register('nav-bar', { require: 'components/nav-bar/nav-bar' });
ko.components.register('home-page', { require: 'components/home-page/home' });
ko.components.register('transaction-list', { require: 'components/transaction-list/tlist' });
ko.components.register('login-page', { require: 'components/login-page/login' });
ko.components.register('register-page', { require: 'components/register-page/register' });
ko.components.register('external-login', { require: 'components/external-login/external-login' });
ko.components.register('invitations', { require: 'components/invitations/invitations' });
ko.components.register('reports', { require: 'components/reports/reports' });

// ... or for template-only components, you can just point to a .html file directly:
ko.components.register('about-page', {
    template: { require: 'text!components/about-page/about.html' }
});

// [Scaffolded component registrations will be inserted here. To retain this feature, don't remove this comment.]

// Start the application
ko.applyBindings(app);
