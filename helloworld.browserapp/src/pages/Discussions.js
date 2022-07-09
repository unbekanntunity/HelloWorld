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
import { Dialog, ReportDialog } from '../components/Dialog';
import MultiInputField from '../components/MultiInputField';
import TopBanner from '../components/TopBanner/TopBanner';

import { sendJSONRequest } from '../requestFuncs';

import "./Discussions.css";

class Discussions extends Component {

    state = {
        discussions: [
            {
                creatorId: this.props.user.id,
                title: "How is ur opinion about MAUI?",
                startMessage: "Hello guys, I spent the last weeks trying out the new MAUI framework. I liked that fact that the MAUI has similar elements to Xamarin, so that wasnt too tough to learn it.By and large, I had a nice experience and recommend to test it out as well.",
                createdAt: "24.05",
                creatorImage: null,
                tags: [
                    {
                        name: "hello"
                    }
                ],
                lastMessage: "Its quite nice, but some things are still...",
                lastMessageAuthor: "HelloWorld",
                lastMessageCreated: "25.05"
            }
        ],
        currentTitle: "",
        currentStartMessage: "",
        showReportDialog: false,
        showStopPreviewButton: false,
        showCreateDiscussionDialog: false,
        existsCreateDiscussionsDialog: true,
    }

    constructor(props) {
        super(props);

        this.tagSectionRef = createRef();
    }

    componentDidMount() {
        this.getDiscussions();
    }

    getDiscussions = () => {
        sendJSONRequest("GET", "/discussion/get_all/", undefined, this.props.tokens.token, {
            CreatorId: this.props.user.id
        }).then(response => {
            this.setState({ discussions: [...this.state.discussions, ...response.data] })
            console.log(response)
        }, error => {
            console.log(error)
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

        sendJSONRequest("POST", "/discussion/create", {
            title: this.state.currentTitle,
            startMessage: this.state.currentStartMessage,
            tagNames: this.tagSectionRef.current.getTags(),
        }, this.props.tokens.token)
            .then(response => {
                if (response.errors) {
                    return;
                }

                this.setState({
                    discussions: [...this.state.discussions, response.data],
                    showCreatePostDialog: false,
                    existsCreatePostDialog: false
                });
            }, error => {
                this.props.onError(error.message);
            })
    }

    handleCreatorInfos = (index) => {
        let newDiscussions = this.state.discussions;

        sendJSONRequest("GET", `/users/get/${this.state.discussions[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newDiscussions[index].creatorImage = response.data.image;
                this.setState({ discussions: newDiscussions })
            }, error => {
                this.props.onError(error.message);
            }); 
    }

    handleStartPreview = () => {
        const previewDiscussions = {
            title: this.state.currentTitle,
            startMessage: this.state.currentStartMessage,
            tags: this.tagSectionRef.current.getTags(),
            creatorImage: this.props.user.imageUrl,
        }

        if (!this.validateDiscussion()) {
            return;
        }

        this.setState({
            savedDiscussions: this.state.discussions,
            discussions: [previewDiscussions],
            showCreateDiscussionDialog: false,
            showStopPreviewButton: true
        })
    }

    handleStopPreview = () => {
        this.setState({
            discussions: this.state.savedDiscussions,
            savedDiscussions: [],
            showCreateDiscussionDialog: true,
            showStopPreviewButton: false,
        })
    }

    handleCreatorInfos = (index) => {
        if (this.state.showStopPreviewButton) {
            return;
        }

        let newDiscussions = this.state.discussions;

        sendJSONRequest("GET", `/users/get/${this.state.discussions[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newDiscussions[index].creatorImage = response.data.image;
                this.setState({
                    discussions: newDiscussions,
                    existsCreateDiscussionsDialog: false,
                    showCreateDiscussionDialog: false,
                })
            }, error => {
                this.props.onError(error.message);
            });
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
                    <div className="discussions-discussions">
                    <div>
                    {
                        this.state.discussions.map((item, index) =>
                            <div key={index} style={{ borderBottom: index !== (this.state.discussions.length - 1) ? "1px solid black" : "none" }}>
                                <Discussion keyProp={index} width={600} onFirstAppear={this.handleCreatorInfos}
                                    title={item.title} startMessage={item.startMessage} createdAt={item.createdAt} tags={item.tags} creatorImage={item.creatorImage}
                                    lastMessage={item.lastMessage} lastMessageAuthor={item.lastMessageAuthor} lastMessageCreated={item.lastMessageCreated}
                                    onFirstAppear={this.handleCreatorInfos} onReportClick={() => this.setState({ showReportDialog: true })}/>
                            </div>
                        )
                    }
                        </div>
                    </div>
                </div>
                <div className="actionMenu">
                    {
                        !this.state.showStopPreviewButton &&
                        <SpeedDial radius={60} iconSize={30} itemFactor={.75}
                            menuOpenedIcon={menuOpened} menuClosedIcon={menuClosed} >
                            <SpeedDial.Item icon={add} onClick={this.handleCreateDiscussion} />
                        </SpeedDial>
                    }
                    {
                        this.state.showStopPreviewButton &&
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
                                    <p className="discussions-multi-label">Description</p>
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
            </div>
        )
    }
}

export default Discussions;