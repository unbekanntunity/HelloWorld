import React, { Component } from 'react';

import edit from '../images/edit.png';

import './Settings.css';

class Settings extends Component {
    state = {
        sections: ["Edit profile", "Change password", "Push notifications", "Privacy", "Help"],
        selectedSection: 0,

        values: {}
    }

    handleChangePic = () => {
        this.setState({
            values: {
                image: ""
            }
        })
    }

    renderSection = () => {
        switch (this.state.selectedSection){
            case 0:
                return (
                    <div>
                        <div className="flex fill">
                            <p className="settings-section-title">Profile picture:</p>
                            <img src={this.props.user.imageUrl} alt="" height={60} width={60} />
                            <img className="settings-section-image-edit" src={edit} alt="" height={20} width={20} onClick={this.handleChangePic} />
                        </div>
                    </div>
                )
        }
    }

    render() {
        return (
            <div className="page-body center-vertical center-horizontal">
                <div className="settings-box">
                    {
                        this.state.sections.map((item, index) =>
                            <div key={index} className="settings-categories-item">
                                <p className="settings-categories-text" onClick={() => this.setState({ selectedSection: index })}>{item}</p>
                            </div>
                        )
                        }
                </div>
                <div className="settings-box settings-category">
                    {
                        this.renderSection()
                    }
                </div>
            </div>

        )
    }
}

export default Settings;