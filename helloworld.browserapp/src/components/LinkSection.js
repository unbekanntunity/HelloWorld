import React, { Component, createRef } from 'react';

import DropDown from './DropDown';
import InputField from './InputField/InputField';

import arrowDown from '../images/down-arrow.png';
import arrowUp from '../images/up-arrow.png';
import github from '../images/github.png';
import discord from '../images/discord.png';
import remove from '../images/delete.png';

import './LinkSection.css';

class LinkSection extends Component {
    state = {
        apps: []
    }

    getLinks = () => {
        return this.state.apps.map(item => {
            return {
                name: item.name,
                value: item.value
            }
        })
    }

    handleInput = (event) => {
        let newApps = this.state.apps;

        const index = this.state.apps.map(item => item.name).indexOf(event.target.name);
        newApps[index].value = event.target.value;

        this.setState({
            apps: newApps
        })
    }

    handleAppSelected = (appName, appIcon, appExample) => {
        if (this.state.apps.map(item => item.name).indexOf(appName) !== -1) {
            return;
        }
        
        this.setState({
            apps: [...this.state.apps, {
                name: appName,
                icon: appIcon,
                example: appExample,
                value: ""
            }]
        })
    }

    handleRemoveApp = (index) => {
        this.setState({
            apps: this.state.apps.filter((_, i) => i !== index)
        })
    }

    render() {
        return (
            <div>
                <div className="section-header">
                    <p className="section-header-text">Links</p>
                    <div className="tagSection-headerDrop">
                        <DropDown toggleButton={{ arrowIconOpen: arrowUp, arrowIconClose: arrowDown }}
                            iconSize={30} arrowIconSize={15} >
                            <DropDown.Item icon={github} text="GitHub" iconSize={30}
                                onClick={() => this.handleAppSelected("Github", github, "https://github.com/unbekanntunity/HelloWorld.git")} />
                            <DropDown.Item icon={discord} text="Discord" iconSize={30}
                                onClick={() => this.handleAppSelected("Discord", discord, "https://discord.gg/EXrmgxHjN9")} />
                        </DropDown>
                    </div>
                </div>
                {
                    this.state.apps.map((item, index) =>
                        <div key={index} className={index !== this.state.apps.length ? "link-section-input link-section-not-last" : "link-section-input" }>
                            <InputField icon={item.icon} iconSize={20} placeholder={item.example} width={400}
                                propName={item.name} onChange={this.handleInput} />
                            <div className="link-section-remove-container">
                                <img className="link-section-remove" src={remove} alt="" width={20} height={20} onClick={() => this.handleRemoveApp(index)} />
                            </div>
                        </div>
                    )
                }
            </div>
        )
    }
}

export default LinkSection;