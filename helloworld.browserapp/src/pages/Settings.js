import React, { Component, createRef } from 'react';
import { Button } from '../components/Button';
import InputField from '../components/InputField/InputField';
import MultiInputField from '../components/MultiInputField';
import TagSection from '../components/TagSection';

import edit from '../images/edit.png';
import text from '../images/title.png';
import { sendFORMRequest, sendJSONRequest } from '../requestFuncs';

import './Settings.css';

class Settings extends Component {
    state = {
        sections: ["Edit profile", "Change password", "Push notifications", "Privacy", "Help"],
        selectedSection: 0,
        values: {},
        user: null,
    }

    constructor(props) {
        super(props);

        this.hiddenInput = createRef();
        this.tagRef = createRef();
    }

    componentDidMount() {
        sendJSONRequest("GET", `/user/get/${this.props.userId}`, undefined, this.props.tokens.token)
            .then(response => {
                this.setState({
                    user: response.data,
                    values: {
                        oldImage: response.data.imageUrl,
                        image: response.data.imageUrl,
                        oldUsername: response.data.userName,
                        username: response.data.userName,
                        oldDescription: response.data.description,
                        description: response.data.description,
                        imageFile: null,
                        oldTags: response.data.tags,
                        tags: response.data.tags
                    }
                })
                this.tagRef.current.updateAddedTags(response.data.tags);
            }, error => {
                this.props.onError(error.message);
            })
    }

    handleChangePic = (event) => {
        var fr = new FileReader();
        fr.onload = () => {
            this.setState({
                values: {
                    ...this.state.values,
                    image: fr.result,
                    imageFile: event.target.files[0]
                }
            })
        };

        fr.readAsDataURL(event.target.files[0]);
    }

    handleSubmit = () => {
        let formData = new FormData();

        formData.append("userName", this.state.values.username);
        formData.append("description", this.state.values.description);
        formData.append("image", this.state.values.imageFile);

        let tags = this.tagRef.current.getTags();
        
        for (var i = 0; i < tags.length; i++) {
            formData.append("tagNames", tags[i]);
        }

        sendFORMRequest("PATCH", "/user/update", formData, this.props.tokens.token)
            .then(response => {
                console.log(response);
                this.setState({
                    values: {
                        ...this.state.values,
                        oldImage: this.state.values.image,
                        oldDescription: this.state.values.description,
                        oldUsername: this.state.values.username

                        }
                    })

                this.props.onUserUpdated();
                this.props.onNotification("Account updated");

            }, error => {
                console.log(error);
                this.props.onError(error.message);
            }
        );
    }

    handleReset = () => {
        this.setState({
            values: {
                ...this.state.values,
                image: this.state.values.oldImage,
                imageFile: null,
                description: this.state.values.oldDescription,
                username: this.state.values.oldUsername
            }
        })
    }

    renderSection = () => {
        switch (this.state.selectedSection){
            case 0:
                return (
                    <div>
                        <div className="settings-section">
                            <p className="settings-section-title">Profile picture:</p>
                            <div className="center-vertical">
                                <img src={this.state.values.image} alt="" height={100} width={100} />
                                <img className="settings-section-image-edit" src={edit} alt="" height={20} width={20} onClick={() => this.hiddenInput.current.click()} />
                                <input ref={this.hiddenInput} className="settings-section-image-input" type="file" accept="image/*" multiple={false}
                                        onInput={this.handleChangePic} />
                            </div>
                        </div>
                        <div className="settings-section">
                            <p className="settings-section-title">Username:</p>
                            <div className="center-vertical">
                                <InputField icon={text} iconSize={20} width={300} height={40} value={this.state.values.username}
                                    onChange={(event) => this.setState({
                                        values: {
                                            ...this.state.values,
                                            username: event.target.value
                                        }
                                    })}/>
                            </div>
                        </div>
                        <div className="settings-section">
                            <p className="settings-section-title">Description:</p>
                            <div className="center-vertical" style={{ padding: "20px 0px 0px 0px" } }>
                                <MultiInputField width={400} height={200} maxLetters={300} value={this.state.values.description}
                                    onChange={(event) => this.setState({
                                            values: {
                                            ...this.state.values,
                                            description: event.target.value
                                        }
                                    })}
                                    />
                            </div>
                        </div>
                        <div className="settings-section">
                            <TagSection ref={this.tagRef} headerSize="1rem" tokens={this.props.tokens} onError={this.props.onError} tagType="Users" initialTags={this.state.values.tags} />
                        </div>
                        <div className="center-horizontal">
                            <div className="settings-section-button" ><Button text="Reset" onClick={this.handleReset} /></div>
                            <div className="settings-section-button" ><Button text="Save" onClick={this.handleSubmit} /></div>
                        </div>
                    </div>
                )
        }
    }

    render() {
        return (
            <div className="page-body center-horizontal">
                <div className="box settings-box">
                    {
                        this.state.sections.map((item, index) =>
                            <div key={index} className={index === this.state.selectedSection ? "settings-categories-item settings-categories-selected" : "settings-categories-item"}
                                onClick={() => this.setState({ selectedSection: index })}>
                                <p className="settings-categories-text">{item}</p>
                            </div>
                        )
                        }
                </div>
                <div className="box settings-box settings-category">
                    {
                        this.renderSection()
                    }
                </div>
            </div>

        )
    }
}

export default Settings;