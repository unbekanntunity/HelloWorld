import React, { Component } from 'react';

import Login from './pages/Login';
import Registration from './pages/Registration';
import Home from './pages/Home';
import Discussions from './pages/Discussions';
import Posts from './pages/Posts';
import Projects from './pages/Projects';
import Account from './pages/Account';

import { Route, Routes, Navigate } from 'react-router-dom';

import { ErrorToast, NotificationToast } from './components/ToastExtensions';
import NavBar from './components/NavBar';

import logo from './images/logo.png';
import message from './images/chat.png';
import binoculars from './images/binoculars.png';
import bell from './images/bell.png';
import arrowDown from './images/down-arrow.png';
import arrowUp from './images/up-arrow.png';
import search from './images/search.png';
import discussion from './images/discussion.png';
import project from './images/laptop.png';
import post from './images/social-media.png';
import account from './images/profile.png';
import bookmark from './images/bookmark.png';
import setting from './images/settings.png';
import exit from './images/exit.png';

import { sendJSONRequest } from './requestFuncs';

import './App.css';
import Settings from './pages/Settings';

class App extends Component {
    state = {
        user: {
            id: "f3680a0b-00cf-488f-a64a-2376143a07bb",
        },
        tokens: {
            token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBZG1pbkBnbWFpbC5jb20iLCJuYW1lIjoiQWRtaW4iLCJqdGkiOiJiNGZiNTNiYS1mZGE0LTRmYzMtOGU3Zi0xM2IyMDFlOWU5NzAiLCJlbWFpbCI6IkFkbWluQGdtYWlsLmNvbSIsImlkIjoiZjM2ODBhMGItMDBjZi00ODhmLWE2NGEtMjM3NjE0M2EwN2JiIiwicm9sZSI6WyJSb290QWRtaW4iLCJVc2VyQWRtaW4iLCJDb250ZW50QWRtaW4iXSwibmJmIjoxNjU3MjU2MzQ4LCJleHAiOjE2OTE4NTUwNDgsImlhdCI6MTY1NzI1NjM0OH0.zO-MEAv0MOa8B3P-YjpjKeHpVnYWFIO3284SGI31WKA"
        },
        errorMessage: "",
        notificationMesasge: "",
        navPages: [
            "/home", "/discussions", "/posts", "/projects", "/account", "/settings"
        ]
    };

    getUser = (token) => {
        sendJSONRequest('GET', `/user/get`, undefined, token)
            .then( response => {
                    console.log(response.data);
                    this.setState({ user: response.data });
            }
        )
    }

    handleSuccessAuthentication = (token, refreshToken) => {
        this.getUser(token);

        this.setState({
            page: "home",
            tokens: {
                token,
                refreshToken
            }
        });

        setTimeout(() => console.log(this.state), 2000);
    }

    handleLogout = () => {
        this.setState({ user: null, tokens: null });
    }

    handleErrorClose = () => {
        this.setState({ hasError: false });
        this.getBackgroundColor();
    }

    handleError = (message) => {
        this.setState({
            errorMessage: message
        });
    }

    handleNotification = (notifcation) => {
        this.setState({
            notificationMesasge: notifcation
        });
    }

    handleErrorClose = () => {
        this.setState({ errorMessage: "" });
    }

    handleNotificationClose = () => {
        this.setState({ notificationMesasge: "" });
    }

    getBackgroundColor = () => {
        if (this.state.page === "login" || this.state.page === "registration") {
            return '#FFFFFFF';
        }
        else {
            return '#F3F2F2';
        }
    }

    isListedUrl = (url) => {
        const index = this.state.navPages.findIndex(element => {
            return element.toLowerCase() === url;
        });

        return index !== -1;
    }

    redirectToPage = (url) => {
        window.location.pathname = url;
    }

    getStartPage = () => {
        return this.state.user === undefined ? <Navigate to="/login" /> : <Navigate to="/home" /> 
    }

    render() {
        return (
            <div id="app-container" style={{ backgroundColor: this.getBackgroundColor() }}>
                {
                    this.isListedUrl(window.location.pathname) &&
                    <NavBar navbar={this.state.navbar}
                        logoLink="/home" logoIcon={logo} searchIcon={search}
                        notificationIcon={bell} exploreIcon={binoculars}
                        arrowUpIcon={arrowUp} arrowDownIcon={arrowDown}
                        onNotificationIconClick={this.handleOnNotificationIconClick}
                        messagesLink="/messages" messagesIcon={message}
                        discussionsLink="/discussions" discussionsIcon={discussion}
                        accountsLink="/accounts" accountsIcon={account}
                        projectsLink="/projects" projectsIcon={project}
                        postsLink="/posts" postsIcon={post}
                        accountLink={`/account/${this.state.user.id}`} accountIcon={account} accountGenIcon={account}
                        savedLink="/saved" savedIcon={bookmark}
                        settingsLink="/settings" settingsIcon={setting}
                        logoutLink="/login" logoutIcon={exit} onLogoutClick={this.props.onLogoutClick}
                    />
                }
                <Routes>
                    <Route path="/" element={this.getStartPage()} />
                    <Route path="/login" element={<Login onLoginSuccess={this.handleSuccessAuthentication} onError={this.handleError} />} />
                    <Route path="/registration" element={<Registration onRegistrationSuccess={this.handleSuccessAuthentication} onError={this.handleError} />} />
                    <Route path="/home" element={<Home tokens={this.state.tokens} sessionUserId={this.state.user.id}
                        onError={this.handleError} onNotifcation={this.handleNotification} onLogOutClick={this.handleLogout} />} />
                    <Route path="/discussions" element={<Discussions onError={this.handleError} tokens={this.state.tokens} user={this.state.user} sessionUserId={this.state.user.id} />} />
                    <Route path="/posts" element={<Posts tokens={this.state.tokens} user={this.state.user} onError={this.handleError} sessionUserId={this.state.user.id} />} />
                    <Route path="/projects" element={<Projects tokens={this.state.tokens} user={this.state.user}
                        onError={this.handleError} onNotification={this.handleNotification} sessionUserId={this.state.user.id} />} />
                    <Route path="/account/:id" element={<Account tokens={this.state.tokens} user={this.state.user} sessionUserId={this.state.user.id}
                        onError={this.handleError} onNotification={this.handleNotification} onSettings={() => this.redirectToPage("/settings")} />} />
                    <Route path="/settings" element={<Settings tokens={this.state.tokens} userId={this.state.user.id}
                        onError={this.handleError} onNotification={this.handleNotification} onUserUpdated={() => this.getUser(this.state.tokens.token)} />} />

                    <Route path="*" element={<h1>404 Error: page does not exist!</h1>}  />
                </Routes>
                <div className="toastContainer">
                    <ErrorToast message={this.state.errorMessage} show={this.state.errorMessage !== ""} onClose={this.handleErrorClose} />
                </div>
                <div className="toastContainer">
                    <NotificationToast message={this.state.notificationMesasge} show={this.state.notificationMesasge !== ""} onClose={this.handleNotificationClose} />
                </div>
            </div>
        );
    }
}

export default App;
