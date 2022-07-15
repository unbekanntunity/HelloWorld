import React, { Component, createRef } from 'react';

import Project from '../components/Project';
import Tag from '../components/Tag';
import TopBanner from '../components/TopBanner/TopBanner';
import Discussion from '../components/Discussion';
import SpeedDial from '../components/SpeedDial';
import { Button, RoundButton } from '../components/Button';
import { Dialog, ReportDialog } from '../components/Dialog';
import { DetailedPost } from '../components/Post';

import InputField from '../components/InputField/InputField';
import MultiInputField from '../components/MultiInputField';
import TagSection from '../components/TagSection';
import ImageSection from '../components/ImageSection';
import LinkSection from '../components/LinkSection';

import menuOpened from '../images/close.png';
import menuClosed from '../images/dots-vertical.png';
import settings from '../images/settings.png';
import post from '../images/social-media.png';
import discussion from '../images/discussion.png';
import project from '../images/laptop.png';
import startPreview from '../images/eye-open.png';
import stopPreview from '../images/eye-close.png';
import title from '../images/title.png';

import { useParams } from "react-router-dom";
import { handleUpdateRating, sendFORMRequest, sendJSONRequest } from '../requestFuncs';

import './Account.css';


class Account extends Component {
    state = {
        user: null,

        items: [],
        selectedIndex: 0,
        selectedCreatedIndex: 0,

        showReportDialog: false,
        showStopPreviewButton: false,
        showCreateItemDialog: false,
        existsCreateItemDialog: true,

        id: "00000000000000000",
        currentTitle: "",
        currentContent: "",
        currentDescription: "",
        currentStartMessage: "",
        tags: [],
        rawImages: [],
        links: [],
        createdAt: "",
    }

    constructor(props) {
        super(props);

        this.tagSectionRef = createRef();
        this.imageSectionRef = createRef();
        this.linkSectionRef = createRef();
    }

    componentDidMount() {
        const { id } = this.props.params;

        this.handleChange(0);

        sendJSONRequest("GET", `/user/get/${id}`, undefined, this.props.tokens.token)
            .then(response => {
                this.setState({ user: response.data })
            }, error => {
                this.props.onError(error.message);
            })
    }

    getUrl = (index, endPoint) => {
        switch (index) {
            case 0:
                return `/post/${endPoint}`
                break;
            case 1:
                return `/discussion/${endPoint}`
            case 2:
                return `/project/${endPoint}`
        }
    }

    handleChange = (index) => {
        let url = this.getUrl(index, "get_all");

        sendJSONRequest("GET", url, undefined, this.props.tokens.token, {
            creatorId: this.props.user.id
        }).then(response => {
            this.setState({ items: [...response.data] })
        }, error => {
            console.log(error);
            this.props.onError(error.message);
        }).finally(() => this.setState({ selectedIndex: index }))
    }

    handleCreatorInfos = (index) => {
        let newItems = this.state.items;
        sendJSONRequest("GET", `/user/get_minimal/${newItems[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newItems[index].creatorImage = response.data.imageUrl;
                newItems[index].creatorName = response.data.userName;
                this.setState({ items: newItems })
            }, error => {
                this.props.onError(error.message);
            });
    }
     
    handleCommentCheck = (creatorId) => {
        return creatorId === this.props.user.id;
    }

    handleSuccessRating = (index, response) => {
        let newItems = this.state.items;
        newItems[index].usersLikedIds = response.data.usersLikedIds;
        this.setState({ items: newItems })
    }

    handleCreateItem = (index) => {
        this.setState({
            existsCreateItemDialog: true,
            showCreateItemDialog: true,
            selectedCreatedIndex: index
        })
    }

    handleStartPreview = () => {
        console.log(this.state.items);

        const previewItem = {
            id: "00000000000000000",
            title: this.state.currentTitle,
            content: this.state.currentContent,
            description: this.state.currentDescription,
            startMessage: this.state.currentStartMessage,
            tags: this.tagSectionRef.current?.getTags(),
            rawImages: this.imageSectionRef.current?.getImages(),
            links: this.linkSectionRef.current?.getLinks(),
            creatorName: this.state.user.userName,
            creatorImage: this.state.user.imageUrl,
            createdAt: new Date().toJSON(),
        }

        if ((this.state.selectedCreatedIndex === 0 && !this.validatePost()) ||
            (this.state.selectedCreatedIndex === 1 && !this.validateDiscussion()) ||
            (this.state.selectedCreatedIndex === 2 && !this.validateProject())) {
            return;
        }

        this.setState({
            savedItems: [...this.state.items],
            items: [],
            showCreateItemDialog: false,
            showStopPreviewButton: true
        })

        setTimeout(() => this.setState({
            items: [previewItem],
            showCreateItemDialog: false,
            showStopPreviewButton: true
        }), 100)
    }

    handleStopPreview = () => {
        this.setState({
            items: [],
            showCreateItemDialog: true,
            showStopPreviewButton: false
        })

        setTimeout(() => this.setState({
            items: [...this.state.savedItems],
            showCreateItemDialog: true,
            showStopPreviewButton: false
        }), 100)
    }

    handleSubmit = () => {
        let formData = new FormData();
        formData.append("title", this.state.currentTitle);
        formData.append("content", this.state.currentContent);
        formData.append("description", this.state.currentDescription);
        formData.append("startMessage", this.state.currentStartMessage);

        const tags = this.tagSectionRef.current?.getTags();
        if (tags) {
            for (var i = 0; i < tags.length; i++) {
                formData.append("tagNames", tags[i]);
            }
        }

        const images = this.imageSectionRef.current?.getImages();
        if (images) {
            for (var i = 0; i < images.length; i++) {
                formData.append("rawImages", images[i]);
            }

        }

        const links = this.linkSectionRef.current?.getLinks();
        if (links) {
            for (var i = 0; i < tags.length; i++) {
                formData.append("links", links[i]);
            }
        }
        

        let url = this.getUrl(this.state.selectedCreatedIndex, "create");
        sendFORMRequest("POST", url, formData, this.props.tokens.token)
            .then(response => {
                console.log(response);
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            }
        )
    }

    validatePost = () => {
        const result = this.state.currentContent.length !== 0 || [...this.imageSectionRef.current.getImages()].length !== 0;

        if (!result) {
            this.props.onError("You can not let the content and the image section empty");
        }

        return result
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

    validateProject = () => {
        let result = this.state.currentName.length !== 0 && this.state.currentDescription.length !== 0

        if (!result) {
            this.props.onError("You can not let the content and the image section empty");
        }

        return result
    }

    renderItems = () => {
        switch (this.state.selectedIndex) {
            case 0:
                return this.state.items.map((item, index) =>
                    <div className="account-item" key={index} >
                        <DetailedPost keyProp={index} id={item.id} creatorId={item.creatorId} creatorImage={item.creatorImage} creatorName={item.creatorName} createdAt={item.createdAt}
                            tags={item.tags} images={item.imageUrls} imageHeight={200} imageWidth={280} text={item.content} width="800px" usersLikedIds={item.usersLikedIds}
                            onFirstAppear={this.handleCreatorInfos} tokens={this.props.tokens} onError={this.props.onError} sessionUserId={this.props.sessionUserId}
                            onReportClick={() => this.setState({ showReportDialog: true })}
                            onLike={(index) => handleUpdateRating(item.id, "post", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                    </div>
                )
            case 1:
                return this.state.items.map((item, index) =>    
                    <div className="account-item" key={index}>
                        <Discussion keyProp={index} width={600} onFirstAppear={this.handleCreatorInfos}
                            title={item.title} startMessage={item.startMessage} createdAt={item.createdAt} tags={item.tags} creatorImage={item.creatorImage}
                            lastMessage={item.lastMessage} lastMessageAuthor={item.lastMessageAuthor} lastMessageCreated={item.lastMessageCreated}
                            onFirstAppear={this.handleCreatorInfos} sessionUserId={this.props.sessionUserId} usersLikedIds={item.usersLikedIds}
                            onLike={(index) => handleUpdateRating(item.id, "discussion", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                    </div>
                )
            case 2:
                return this.state.items.map((item, index) =>
                    <div className="account-item" key={index} >
                        <Project keyProp={index} title={item.title} createdAt={item.createdAt} description={item.description} creatorId={item.creatorId}
                            images={item.images} creatorImage={item.creatorImage} tags={item.tags} width={600} imageHeight={300} imageWidth={500}
                            onReportClick={() => this.setState({ showReportDialog: true })} sessionUserId={this.props.sessionUserId} usersLikedIds={item.usersLikedIds}
                            onFirstAppear={this.handleCreatorInfos}
                            onLike={(index) => handleUpdateRating(item.id, "project", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                    </div>
                )
        }
    }

    renderCreateDialog = () => {
        switch (this.state.selectedCreatedIndex) {
            case 0:
                return (
                    <div className="dialog-container" style={{ display: this.state.showCreateItemDialog ? 'inherit' : 'none' }} >
                        <Dialog title="Create new post" height="fit-content" width="400px" paddingX="20px" paddingY="20px"
                            onBackClick={() => this.setState({
                                showCreateItemDialog: false,
                                existsCreateItemDialog: false,
                            })} rightIcon={startPreview} onRightClick={this.handleStartPreview} backButton={true}>
                            <TagSection ref={this.tagSectionRef} tokens={this.props.tokens} onError={this.props.onError} zIndex={2} tagType="Posts" />
                            <div className="posts-multi-container">
                                <p className="posts-multi-label">Content</p>
                                <MultiInputField placeholder="" height="200px" maxLetters={this.state.maxLetters}
                                    onChange={(event) => this.setState({ currentContent: event.target.value })} />
                            </div>
                            <div className="posts-imageSection">
                                <ImageSection ref={this.imageSectionRef} imageSize={40} />
                            </div>
                            <div className="posts-button-container">
                                <Button text="Create" onClick={this.handleSubmit} />
                            </div>
                        </Dialog>
                    </div>
                )
            case 1:
                return (
                    <div className="dialog-container" style={{ display: this.state.showCreateItemDialog ? 'inherit' : 'none' }} >
                        <Dialog title="Create new discussion" height="fit-content" width="600px" paddingX="20px" paddingY="20px"
                            onBackClick={() => this.setState({
                                showCreateItemDialog: false,
                                existsCreateItemDialog: false,
                            })} backButton={true} rightIcon={startPreview} onRightClick={this.handleStartPreview}>
                            <InputField icon={title} iconSize={20} placeholder="Title" width={300}
                                onChange={(event) => this.setState({ currentTitle: event.target.value })} />
                            <div className="discussions-tags">
                                <TagSection ref={this.tagSectionRef} tokens={this.props.tokens} onError={this.props.onError} zIndex={2} tagType="Discussions" />
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
                )
            case 2:
                return (
                    <div className="dialog-container" style={{ display: this.state.showCreateItemDialog ? 'inherit' : 'none' }} >
                        <Dialog title="Create new project" height="fit-content" width={500} paddingX="20px" paddingY="20px"
                            onBackClick={() => this.setState({
                                showCreateItemDialog: false,
                                existsCreateItemDialog: false,
                            })} rightIcon={startPreview} onRightClick={this.handleStartPreview} backButton={true}>
                            <InputField icon={title} iconSize={20} placeholder="Project name" width={300}
                                onChange={(event) => this.setState({ currentName: event.target.value })} />
                            <div className="create-multi-container">
                                <p className="discussions-multi-label">Description</p>
                                <MultiInputField placeholder="" height="200px" maxLetters={300}
                                    onChange={(event) => this.setState({ currentDescription: event.target.value })} zindex={1} />
                            </div>
                            <TagSection ref={this.tagSectionRef} tokens={this.props.tokens} onError={this.props.onError} zIndex={2} tagType="Projects" />
                            <ImageSection ref={this.imageSectionRef} imageSize={40} />
                            <LinkSection ref={this.linkSectionRef} />
                            <div className="center-horizontal">
                                <Button text="Create" onClick={this.handleSubmit} />
                            </div>
                        </Dialog>
                    </div>    
                )
        }
    }

    render() {
        return (
            <div className="page-body">
                <div className="actionMenu">
                    {
                        !this.state.showStopPreviewButton &&
                        <SpeedDial radius={60} iconSize={30} itemFactor={.75} menuOpenedIcon={menuOpened} menuClosedIcon={menuClosed} >
                            <SpeedDial.Item icon={post} onClick={() => this.handleCreateItem(0)} />
                            <SpeedDial.Item icon={discussion} onClick={() => this.handleCreateItem(1)} />
                            <SpeedDial.Item icon={project} onClick={() => this.handleCreateItem(2)} />
                        </SpeedDial>
                    }
                    {
                        this.state.showStopPreviewButton &&
                        <RoundButton icon={stopPreview} radius={60} iconSize={30} onClick={this.handleStopPreview} />
                    }
                </div>
                {
                    this.state.user &&
                    <div className="account-profile-section">
                        <div className="account-pic-name">
                            <img src={this.state.user.imageUrl} alt="" width={100} height={100} />
                            <p className="account-name">{this.state.user.userName}</p>
                        </div>
                        <div className="account-description-container">
                            <p className="account-description">Description</p>
                            <p>{this.state.user.description}</p>
                        </div>
                        <div className="account-tags">
                            {
                                this.state.user.tags.map((item, index) =>
                                    <Tag key={index} name={item} fontSize="20px" margin="10px" />
                                )
                            }
                        </div>
                        <div className="account-edit-container">
                            <img src={settings} alt="" height={30} width={30} onClick={this.props.onSettings} />
                        </div>
                    </div>
                }
                <TopBanner bgColor="#F3F2F2" onSelectionChanged={this.handleChange}>
                    <TopBanner.UnderlineItem name="Posts" selectedBorderColor="#FF9900" textColor="black"/>
                    <TopBanner.UnderlineItem name="Discussions" selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name="Projects" selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name="Saved" selectedBorderColor="#FF9900" textColor="black" />
                </TopBanner>
                {
                    <div className="account-items">
                    {
                        this.renderItems()
                    }
                    </div>
                }
                {
                    this.state.existsCreateItemDialog &&
                    this.renderCreateDialog()
                }
                {
                    this.state.showReportDialog &&
                    <ReportDialog onClose={() => this.setState({ showReportDialog: false })} onNotifcation={this.props.onNotifcation} />
                }
            </div>
        )
    }
}

function withParams(Component) {
    return props => <Component {...props} params={useParams()} />;
}

export default withParams(Account);