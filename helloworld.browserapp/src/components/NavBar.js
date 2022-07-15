import React, { Component } from 'react';
import { Nav } from 'react-bootstrap';

import InputField from './InputField/InputField';
import DropDown from './DropDown';

import './NavBar.css';

class NavBar extends Component {
    render() {
        return (
            <Nav className="navbar" style={{ boxShadow: '0px 1px 18px #FFB30B'}}  >
                <a className="navbar-brand" href={this.props.logoLink}>
                    <img src={this.props.logoIcon} width="90" height="60" alt="" />
                </a>
                <InputField className="searchBar" icon={this.props.searchIcon} iconSize='4%' width='500px' height="40px"/>
                <div className="buttons">
                    <Nav.Item>
                        <Nav.Link href={this.props.messagesLink}>
                            <img src={this.props.messagesIcon} width={30} height={30} alt="" />
                        </Nav.Link>
                    </Nav.Item>
                    <Nav.Item>
                        <div style={{ marginRight: "10px" }}>
                            <DropDown toggleButton={{ icon: this.props.exploreIcon, arrowIconOpen: this.props.arrowUpIcon, arrowIconClose: this.props.arrowDownIcon }}
                                iconSize={30} arrowIconSize={15} >
                                <DropDown.Item linkTo={this.props.discussionsLink} icon={this.props.discussionsIcon} text="Discussions" iconSize={35}/>
                                <DropDown.Item linkTo={this.props.projectsLink} icon={this.props.projectsIcon} text="Projects" iconSize={30}/>
                                <DropDown.Item linkTo={this.props.postsLink} icon={this.props.postsIcon} text="Posts" iconSize={30} />
                                <DropDown.Item linkTo={this.props.accountsLink} icon={this.props.accountsIcon} text="Accounts" iconSize={30} />
                            </DropDown>
                        </div>
                    </Nav.Item>
                    <Nav.Item>
                        <DropDown toggleButton={{ icon: this.props.notificationIcon }} iconSize={30} arrowIconSize={15} contentWidth="300px" contentLeft="220px" />
                    </Nav.Item>
                    <Nav.Item>
                        <div className="account-icon">
                            <DropDown toggleButton={{ icon: this.props.accountIcon, }} iconSize={40} arrowIconSize={15} contentLeft="100px" zIndex={3}>
                                <DropDown.Item linkTo={this.props.accountLink} icon={this.props.accountGenIcon} text="Profile" iconSize={35} />
                                <DropDown.Item linkTo={this.props.savedLink} icon={this.props.savedIcon} text="Saved" iconSize={30} />
                                <DropDown.Item linkTo={this.props.settingsLink} icon={this.props.settingsIcon} text="Settings" iconSize={30} />
                                <DropDown.Item linkTo={this.props.logoutLink} icon={this.props.logoutIcon} text="Log out" iconSize={30}
                                    onClick={this.props.onLogoutClick} />
                            </DropDown>
                        </div>
                    </Nav.Item>
                </div>
            </Nav>
        );
    };
}

export default NavBar;
