import React, { Component, createRef } from 'react';

import menuOpened from '../images/close.png';
import menuClosed from '../images/dots-vertical.png';
import add from '../images/plus.png';
import startPreview from '../images/eye-open.png';
import stopPreview from '../images/eye-close.png';
import title from '../images/title.png';

import InputField from '../components/InputField/InputField';
import TagSection from '../components/TagSection';
import Discussion from '../components/Discussion';
import LeftBanner from '../components/LeftBanner';
import SpeedDial from '../components/SpeedDial';
import { RoundButton, Button } from '../components/Button';
import { DeleteConfirmDialog, Dialog, ReportDialog } from '../components/Dialog';
import MultiInputField from '../components/MultiInputField';
import TopBanner from '../components/TopBanner/TopBanner';

import { handleUpdateRating, sendFORMRequest, sendJSONRequest } from '../requestFuncs';

import "./Discussions.css";

class Discussions extends Component {

    state = {
        discussions: [],
        currentTitle: "",
        currentStartMessage: "",
        showReportDialog: false,
        showCreateDiscussionDialog: false,
        existsCreateDiscussionsDialog: true,
        previewMode: false,

        showDeleteConfirmDialog: false,
        currentDeleteItemIndex: null,
    }

    constructor(props) {
        super(props);

        this.tagSectionRef = createRef();
    }

    componentDidMount() {
        this.getDiscussions();
    }

    getDiscussions = () => {
        sendJSONRequest("GET", "/discussion/get_all/", undefined, this.props.tokens.token)
            .then(response => {
            this.setState({ discussions: [...this.state.discussions, ...response.data] })
        }, error => {
            this.props.onError(error.message)
        })
    }

    handleCreateDiscussion = () => {
        this.setState({
            showCreateDiscussionDialog: true,
            existsCreateDiscussionDialog: true
        })
    }

    handleSubmit = () => {
        if (!this.validateDiscussion()) {
            return;
        }

        // In case of image support
        let formData = new FormData();
        formData.append("title", this.state.currentTitle);
        formData.append("startMessage", this.state.currentStartMessage);

        let tags = this.tagSectionRef.current.getTags();
        for (let i = 0; i < tags.length; i++) {
            formData.append("tagNames", tags[i]);
        }

        sendJSONRequest("POST", "/discussion/create", {
            title: this.state.currentTitle,
            startMessage: this.state.currentStartMessage,
            tagNames: tags
        }, this.props.tokens.token)
            .then(response => {
                if (response.errors) {
                    return;
                }

                this.setState({
                    discussions: [...this.state.discussions, response.data],
                    showCreateDiscussionDialog: false,
                    existsCreateDiscussionsDialog: false
                });
            }, error => {
                this.props.onError(error.message);
            })
    }

    handleStartPreview = () => {
        const previewDiscussions = {
            title: this.state.currentTitle,
            startMessage: this.state.currentStartMessage,
            tags: this.tagSectionRef.current.getTags(),
            creatorImage: this.props.user.imageUrl,
            createdAt: Date.now()
        }

        if (!this.validateDiscussion()) {
            return;
        }

        this.setState({
            savedDiscussions: this.state.discussions,
            discussions: [previewDiscussions],
            showCreateDiscussionDialog: false,
            previewMode: true,
        })
    }

    handleStopPreview = () => {
        this.setState({
            discussions: this.state.savedDiscussions,
            savedDiscussions: [],
            showCreateDiscussionDialog: true,
            previewMode: false,
        })
    }

    handleCreatorInfos = (index) => {
        if (this.state.showStopPreviewButton) {
            return;
        }

        let newDiscussions = this.state.discussions;

        sendJSONRequest("GET", `/user/get_minimal/${this.state.discussions[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newDiscussions[index].creatorImage = response.data.imageUrl;
                this.setState({
                    discussions: newDiscussions,
                    existsCreateDiscussionsDialog: false,
                    showCreateDiscussionDialog: false,
                })
            }, error => {
                this.props.onError(error.message);
            });
    }

    handleSuccessRating = (index, response) => {
        let newDiscussions = this.state.discussions;
        newDiscussions[index].usersLikedIds = response.data.usersLikedIds;
        this.setState({ discussions: newDiscussions })
    }

    handleDelete = () => {
        let id = this.state.discussions[this.state.currentDeleteItemIndex].id;
        sendJSONRequest("DELETE", `/discussion/delete/${id}`, undefined, this.props.tokens.token)
            .then(_ => {
                this.setState({
                    discussions: this.state.discussions.filter((_, index) => index !== this.state.currentDeleteItemIndex),
                    currentDeleteItemIndex: null,
                    showDeleteConfirmDialog: false
                });

                this.props.onNotification("Item successfully removed");
                }, error => {
                    console.log(error);
                    this.props.onError(error.message);
                }
            )
    }

    validateDiscussion = () => {
        let result = this.state.currentTitle.length !== 0 && this.state.currentTitle.length !== 0;

        if (this.state.currentTitle.length === 0) {
            this.props.onError("You have to add a title");
        }
        if (this.state.currentStartMessage.length === 0) {
            this.props.onError("You have to add a start message");
        }

        return result
    }

    render() {
        return (
            <div className="page-body flex">
                <div>
                    <LeftBanner text="Discussions" />
                </div>
                <div className="discussions-body-main">
                    <TopBanner bgColor="#ffd374">
                        <TopBanner.SimpleItem name="New" selectedTextColor="white" unselectedTextColor="black" />
                        <TopBanner.SimpleItem name="Followed" selectedTextColor="white" unselectedTextColor="black" />
                        <TopBanner.SimpleItem name="Active" selectedTextColor="white" unselectedTextColor="black" />
                    </TopBanner>
                    <div className="center-vertical column fill">
                    <div>
                    {
                        this.state.discussions.map((item, index) =>
                            <div key={index} style={{ borderBottom: index !== (this.state.discussions.length - 1) ? "1px solid black" : "none" }}>
                                <Discussion keyProp={index} width={600} item={item} sessionUserId={this.props.sessionUserId} previewMode={this.state.previewMode}
                                    onFirstAppear={this.handleCreatorInfos} onReportClick={() => this.setState({ showReportDialog: true })}
                                    onDelete={(index) => this.setState({
                                        showDeleteConfirmDialog: true,
                                        currentDeleteItemIndex: index
                                    })}
                                    onLike={(index) => handleUpdateRating(item.id, "discussion", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                            </div>
                        )
                    }
                        </div>
                    </div>
                </div>
                <div className="actionMenu">
                    {
                        !this.state.previewMode &&
                        <SpeedDial radius={60} iconSize={30} itemFactor={.75}
                            menuOpenedIcon={menuOpened} menuClosedIcon={menuClosed} >
                            <SpeedDial.Item icon={add} onClick={this.handleCreateDiscussion} />
                        </SpeedDial>
                    }
                    {
                        this.state.previewMode &&
                        <RoundButton icon={stopPreview} radius={60} iconSize={30} onClick={this.handleStopPreview} />
                    }
                </div>
                {
                    this.state.existsCreateDiscussionDialog &&
                    <div className="dialog-container" style={{ display: this.state.showCreateDiscussionDialog ? 'inherit' : 'none' }} >
                            <Dialog title="Create new discussion" height="fit-content" width="600px" paddingX="20px" paddingY="20px"
                                onBackClick={() => this.setState({
                                    showCreateDiscussionDialog: false,
                                    existsCreateDiscussionDialog: false,
                                })} backButton={true} rightIcon={startPreview} onRightClick={this.handleStartPreview}>
                                <InputField icon={title} iconSize={20} placeholder="Title" width={300}
                                    onChange={(event) => this.setState({ currentTitle: event.target.value })} />
                                <div className="discussions-tags">
                                    <TagSection ref={this.tagSectionRef} tokens={this.props.tokens} onError={this.props.onError} zIndex={2} tagType="Discussions"/>
                                </div>
                                <div className="discussions-multi-container">
                                    <p className="discussions-multi-label">Start message</p>
                                    <MultiInputField placeholder="" height="200px" maxLetters={300}
                                        onChange={(event) => this.setState({ currentStartMessage: event.target.value })} zindex={1} />
                                </div>
                                <div className="discussions-button-container">
                                    <Button text="Create" onClick={this.handleSubmit} />
                                </div>
                            </Dialog>
                    </div>
                }
                {
                    this.state.showReportDialog &&
                    <ReportDialog onClose={() => this.setState({ showReportDialog: false })} onNotifcation={this.props.onNotifcation} />
                }
                {
                    this.state.showDeleteConfirmDialog &&
                    <DeleteConfirmDialog onBack={() => this.setState({
                        showDeleteConfirmDialog: false,
                        currentDeleteItemIndex: null
                    })} onCancel={() => this.setState({
                        showDeleteConfirmDialog: false,
                        currentDeleteItemIndex: null
                    })} onConfirm={this.handleDelete} />
                }
            </div>
        )
    }
}

export default Discussions;