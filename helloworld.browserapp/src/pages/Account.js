import React, { Component, createRef } from 'react';

import { DetailedProject, Project } from '../components/Project';
import Tag from '../components/Tag';
import TopBanner from '../components/TopBanner/TopBanner';
import Discussion from '../components/Discussion';
import SpeedDial from '../components/SpeedDial';
import { Button, RoundButton } from '../components/Button';
import { DeleteConfirmDialog, Dialog, ReportDialog, UsersDialog } from '../components/Dialog';
import { DetailedPost, Post } from '../components/Post';

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
import follow from '../images/follow.png';
import followed from '../images/followed.png';

import { useParams } from "react-router-dom";
import { handleUpdateRating, sendFORMRequest, sendJSONRequest } from '../requestFuncs';

import './Account.css';
import MembersSection from '../components/MembersSection';


class Account extends Component {
    state = {
        user: null,

        items: [],
        selectedIndex: 0,
        selectedCreatedIndex: 0,
        selectedSection: 0,
        currentDeleteItemIndex: null,

        showReportDialog: false,
        showFollowersDialog: false,
        showStopPreviewButton: false,
        showCreateItemDialog: false,
        existsCreateItemDialog: true,
        showDeleteConfirmDialog: false,
        showEditItemDialog: false,
        existsEditItemDialog: false,

        id: "",
        currentTitle: "",
        currentContent: "",
        currentDescription: "",
        currentStartMessage: "",
    }

    constructor(props) {
        super(props);

        this.tagSectionRef = createRef();
        this.imageSectionRef = createRef();
        this.linkSectionRef = createRef();
        this.membersSectionRef = createRef();
    }

    componentDidMount() {
        const { id } = this.props.params;

        sendJSONRequest("GET", `/user/get/${id}`, undefined, this.props.tokens.token)
            .then(response => {
                console.log(response);
                this.setState({ user: response.data })

                setTimeout(() => {
                    this.handleChange(0);
                }, 100);
                
            }, error => {
                this.props.onError(error.message);
            })

    }

    getUrl = (index, endPoint, id = null) => {
        switch (index) {
            case 0:
                return id ? `/post/${endPoint}/${id}` : `/post/${endPoint}`
                break;
            case 1:
                return id ? `/discussion/${endPoint}/${id}` : `/discussion/${endPoint}`
            case 2:
                return id ? `/project/${endPoint}/${id}` : `/project/${endPoint}`
        }
    }

    handleChange = (index) => {
        this.setState({
            items: index === 0 ? this.state.user.posts :
                index === 1 ? this.state.user.discussions :
                    this.state.user.projects,
            selectedIndex: index
        })
    }

    handleCreatorInfosForSaved = (index) => {
        const category = this.state.selectedSection === 0 ? "savedPosts" :
            this.state.selectedSection === 1 ? "savedDiscussions" : "savedProjects";

        let newSavedItems = this.state.user[category];
        sendJSONRequest("GET", `/user/get_minimal/${newSavedItems[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newSavedItems[index].creatorImage = response.data.imageUrl;
                newSavedItems[index].creatorName = response.data.userName;
                this.setState({
                    user: {
                        ...this.state.user,
                        [category]: newSavedItems
                    }})
            }, error => {
                this.props.onError(error.message);
            });
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
            previewMode: true,
            id: "00000000000000000",
            creatorId: this.state.user.id,
            title: this.state.currentTitle,
            content: this.state.currentContent,
            description: this.state.currentDescription,
            startMessage: this.state.currentStartMessage,
            tags: this.tagSectionRef.current?.getTags(),
            rawImages: this.imageSectionRef.current?.getImages(),
            links: this.linkSectionRef.current?.getLinks(),
            memberIds: this.membersSectionRef.current?.getMembers(),
            usersLikedIds: [],
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

    collectData = () => {
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
            for (var i = 0; i < links.length; i++) {
                formData.append("links", JSON.stringify(links[i]));
            }
        }

        const members = this.membersSectionRef.current?.getMembers();
        if (members) {
            for (var i = 0; i < members.length; i++) {
                formData.append("memberIds", members[i]);
            }
        }

        return formData;
    }

    handleSubmit = () => {
        let formData = this.collectData();

        let url = this.getUrl(this.state.selectedCreatedIndex, "create");
        sendFORMRequest("POST", url, formData, this.props.tokens.token)
            .then(response => {
                console.log(response);
                this.setState({
                    items: [...this.state.items, response.data],
                    currentTitle: "",
                    currentContent: "",
                    currentDescription: "",
                    currentStartMessage: "",

                    showCreateItemDialog: false,
                    existsCreateItemDialog: false
                })
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            }
        )
    }

    handleEdit = async (index, project) => {
        this.setState({
            existsEditItemDialog: true,
            showEditItemDialog: true,

            id: project.id,
            currentTitle: project.title,
            currentDescription: project.description,
        })

        setTimeout(() => {
            this.tagSectionRef.current.updateAddedTags(project.tags);
        }, 100)
    }

    handleSubmitEdit = () => {
        let formData = this.collectData();
        let url = this.getUrl(this.state.selectedCreatedIndex, "update", this.state.id);
        sendFORMRequest("PATCH", url, formData, this.props.tokens.token)
            .then(response => {
                console.log(response);
                this.setState({
                    items: [...this.state.items, response.data],
                    id: "",
                    title: "",
                    content: "",
                    description: "",
                    startMessage: "",

                    showCreateItemDialog: false,
                    existsCreateItemDialog: false
                })
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            }
            )
    }

    handleFollow = () => {
        sendJSONRequest("PATCH", `/user/update_following/${this.state.user.id}`, undefined, this.props.tokens.token)
            .then(response => {
                this.setState({
                    user: {
                        ...this.state.user,
                        followerIds: response.data.followerIds
                    }
                });
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            }
        )
    }

    handleDelete = () => {
        const id = this.state.items[this.state.currentDeleteItemIndex].id;
        const url = this.getUrl(this.state.selectedIndex, "delete", id);
        const category = this.state.selectedSection === 0 ? "savedPosts" :
            this.state.selectedSection === 1 ? "savedDiscussions" : "savedProjects";


        sendJSONRequest("DELETE", url, undefined, this.props.tokens.token)
            .then(_ => {
                this.setState({
                    items: this.state.items.filter((_, item_index) => item_index !== this.state.currentDeleteItemIndex),
                    user: {
                        ...this.state.user,
                        [category]: this.state.user[category].filter(item => item.id !== id)
                    },
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

    handleSave = (id) => {
        const url = this.getUrl(this.state.selectedIndex, "update_saving", id);

        const category = this.state.selectedIndex === 0 ? "savedPosts" :
            this.state.selectedIndex === 1 ? "savedDiscussions" : "savedProjects";

        sendJSONRequest("PATCH", url, undefined, this.props.tokens.token)
            .then(response => {
                this.setState({
                    user: {
                        ...this.state.user,
                        [category]: [...response.data]
                    }
                });

                this.props.onNotification("Item successfully saved");
            }, error => {
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
        let result = this.state.currentTitle.length !== 0 && this.state.currentDescription.length !== 0

        if (!result) {
            this.props.onError("You can not let the content and the image section empty");
        }

        return result
    }

    renderSections = () => {
        switch (this.state.selectedSection) {
            case 0: 
                return this.state.user.savedPosts.length > 0 ? (
                    <div className="account-saved-grid">
                        {
                            this.state.user.savedPosts.map((item, index) =>
                                <Post key={index} keyProp={index} creatorImage={item.creatorImage} creatorId={item.creatorId} creatorName={item.creatorName} createdAt={item.createdAt}
                                    tags={item.tags} images={item.imageUrls} imageHeight={200} imageWidth={280} text={item.content} width="300px"
                                    sessionUserId={this.props.sessionUserId} usersLikedIds={item.usersLikedIds}
                                    onDelete={(index) => this.setState({
                                        showDeleteConfirmDialog: true,
                                        currentDeleteItemIndex: index
                                    })} saved={this.state.user.savedPosts.map(item => item.id).indexOf(item.id) !== -1} onSave={this.handleSave}
                                    onFirstAppear={this.handleCreatorInfosForSaved} onReportClick={() => this.setState({ showReportDialog: true })} />
                            )
                        }
                    </div>
                ) : (<p>No posts saved</p>)
            case 1:
                return this.state.user.savedDiscussions.length > 0 ? (
                    <div className="account-saved-list">
                        {
                            this.state.user.savedDiscussions.map((item, index) =>
                                <Discussion keyProp={index} width={600} usersLikedIds={item.usersLikedIds} creatorId={item.creatorId}
                                    title={item.title} startMessage={item.startMessage} createdAt={item.createdAt} tags={item.tags} creatorImage={item.creatorImage} creatorId={item.creatorId}
                                    lastMessage={item.lastMessage} lastMessageAuthor={item.lastMessageAuthor} lastMessageCreated={item.lastMessageCreated} sessionUserId={this.props.sessionUserId}
                                    onFirstAppear={this.handleCreatorInfos} onReportClick={() => this.setState({ showReportDialog: true })}
                                    onSave={this.handleSave}  onDelete={(index) => this.setState({
                                        showDeleteConfirmDialog: true,
                                        currentDeleteItemIndex: index
                                    })} onFirstAppear={this.handleCreatorInfosForSaved} saved={this.state.user.savedDiscussions.map(item => item.id).indexOf(item.id) !== -1}
                                    onLike={(index) => handleUpdateRating(item.id, "discussion", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                            )
                        }
                    </div>
                ) : (<p>No discussions saved</p>)
            case 2:
                return this.state.user.savedProjects.length > 0 ? (
                    <div className="account-saved-list">
                        {
                            this.state.user.savedProjects.map((item, index) =>
                                <Project key={index} keyProp={index} title={item.title} createdAt={item.createdAt} description={item.description} usersLikedIds={item.usersLikedIds}
                                    images={item.imageUrls} creatorImage={item.creatorImage} tags={item.tags} width={600} imageHeight={300} imageWidth={500} memberIds={item.memberIds}
                                    onReportClick={() => this.setState({ showReportDialog: true })} onFirstAppear={this.handleCreatorInfos} sessionUserId={this.props.sessionUserId}
                                    onSave={this.handleSave} onDelete={(index) => this.setState({
                                        showDeleteConfirmDialog: true,
                                        currentDeleteItemIndex: index
                                    })} onFirstAppear={this.handleCreatorInfosForSaved} creatorId={item.creatorId} saved={this.state.user.savedProjects.map(item => item.id).indexOf(item.id) !== -1}
                                    onLike={(index) => handleUpdateRating(item.id, "project", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                            )
                        }
                    </div>
                ) : (<p>No projects saved</p>)
        }
    }

    renderItems = () => {
        switch (this.state.selectedIndex) {
            case 0:
                return this.state.items.map((item, index) => 
                    <div className="account-item" key={index} >
                        <DetailedPost keyProp={index} item={item} imageHeight={200} imageWidth={280} width="800px"
                            onFirstAppear={this.handleCreatorInfos} tokens={this.props.tokens} onError={this.props.onError} sessionUserId={this.props.sessionUserId}
                            onReportClick={() => this.setState({ showReportDialog: true })}
                            onDelete={(index) => this.setState({
                                showDeleteConfirmDialog: true,
                                currentDeleteItemIndex: index
                            })} onSave={this.handleSave} saved={this.state.user.savedPosts.map(item => item.id).indexOf(item.id) !== -1}
                            onLike={(index) => handleUpdateRating(item.id, "post", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                    </div>
                )
            case 1:
                return this.state.items.map((item, index) =>    
                    <div className="account-item" key={index}>
                        <Discussion keyProp={index} width={600} item={item} saved={this.state.user.savedDiscussions.map(item => item.id).indexOf(item.id) !== -1}
                            onFirstAppear={this.handleCreatorInfos} sessionUserId={this.props.sessionUserId}
                            onDelete={(index) => this.setState({
                                showDeleteConfirmDialog: true,
                                currentDeleteItemIndex: index
                            })} onSave={this.handleSave} saved={this.state.user.savedDiscussions.map(item => item.id).indexOf(item.id) !== -1}
                            onLike={(index) => handleUpdateRating(item.id, "discussion", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                    </div>
                )
            case 2:
                console.log(this.state.items);
                return this.state.items.map((item, index) =>
                    <div className="account-item" key={index} >
                        <DetailedProject keyProp={index} tokens={this.props.tokens} item={item} width={600} imageHeight={300} imageWidth={500}
                            sessionUserId={this.props.sessionUserId} onEdit={this.handleEdit}
                            onFirstAppear={this.handleCreatorInfos} saved={this.state.user.savedPosts.map(item => item.id).indexOf(item.id) !== -1} 
                            onDelete={(index) => this.setState({
                                showDeleteConfirmDialog: true,
                                 currentDeleteItemIndex: index
                             })} onReportClick={() => this.setState({ showReportDialog: true })} onSave={this.handleSave} saved={this.state.user.savedProjects.map(item => item.id).indexOf(item.id) !== -1}
                            onLike={(index) => handleUpdateRating(item.id, "project", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                    </div>
                )
            case 3:
                let sections = ["Posts", "Discussions", "Projects"]

                return (
                    <div className="fill center-horizontal">
                        <div className="box settings-box">
                            {
                                sections.map((item, index) =>
                                    <div key={index} className={index === this.state.selectedSection ? "settings-categories-item settings-categories-selected" : "settings-categories-item"}
                                        onClick={() => this.setState({ selectedSection: index })}>
                                        <p className="settings-categories-text">{item}</p>
                                    </div>
                                )
                            }
                        </div>
                        <div className="box account-saved-category">
                        {
                            this.renderSections()
                        }
                        </div>
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
                        <Dialog title="Create new project" height={600} width={700} paddingX="20px" paddingY="20px"
                            onBackClick={() => this.setState({
                                showCreateItemDialog: false,
                                existsCreateItemDialog: false,
                            })} rightIcon={startPreview} onRightClick={this.handleStartPreview} backButton={true}>
                            <div className="flex">
                                <div>
                                    <InputField icon={title} iconSize={20} placeholder="Project name" width={300}
                                        onChange={(event) => this.setState({ currentTitle: event.target.value })} />
                                    <div className="create-multi-container">
                                        <p>Description</p>
                                        <MultiInputField placeholder="" height={200} maxLetters={300} width={300}
                                            onChange={(event) => this.setState({ currentDescription: event.target.value })} zindex={1} />
                                    </div>
                                </div>
                                <div className="account-members-section fill">
                                    <MembersSection ref={this.membersSectionRef} tokens={this.props.tokens} inputWidth={250} />
                                </div>
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
                    this.state.user !== null &&
                    <div className="account-profile-section">
                        <div className="account-pic-name">
                            <img src={this.state.user.imageUrl} alt="" width={100} height={100} />
                                <p className="account-name">{this.state.user.userName}</p>
                                <p className="account-follower" onClick={() => this.setState({ showFollowersDialog: true })}>
                                    Followers: {this.state.user.followerIds.length}
                                </p>
                        </div>
                        <div className="account-description-container">
                            <p className="account-description">Description</p>
                                <p>{this.state.user.description ? this.state.user.description : "This user is too lazy to provide a description :("}</p>
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
                            {
                                this.props.sessionUserId !== this.state.user.id &&
                                <img src={this.state.user.followerIds.indexOf(this.props.sessionUserId) !== -1 ? followed : follow}
                                    alt="" height={30} width={30} onClick={this.handleFollow} />

                            }
                        </div>
                    </div>
                }
                <TopBanner bgColor="#F3F2F2" onSelectionChanged={this.handleChange}>
                    <TopBanner.UnderlineItem name={`Posts${this.state.selectedIndex === 0 ? `(${this.state.items.length})` : ""}`}
                        selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name={`Discussions${this.state.selectedIndex === 1 ? `(${this.state.items.length})` : ""}`}
                        selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name={`Projects${this.state.selectedIndex === 2 ? `(${this.state.items.length})` : ""}`}
                        selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name={`Saved${this.state.selectedIndex === 3 ? `(${this.state.items.length})` : ""}`}
                        selectedBorderColor="#FF9900" textColor="black" />
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
                    this.state.existsEditItemDialog &&
                    <div className="dialog-container" style={{ display: this.state.showEditItemDialog ? 'inherit' : 'none' }} >
                        <Dialog title="Edit project" height={600} width={700} paddingX="20px" paddingY="20px"
                            onBackClick={() => this.setState({
                                showEditItemDialog: false,
                                existsEditItemDialog: false,
                            })} rightIcon={startPreview} onRightClick={this.handleStartPreview} backButton={true}>
                            <div className="flex">
                                    <div>
                                        <InputField icon={title} iconSize={20} placeholder="Project name" width={300} value={this.state.currentTitle}
                                        onChange={(event) => this.setState({ currentTitle: event.target.value })} />
                                    <div className="create-multi-container">
                                        <p>Description</p>
                                            <MultiInputField placeholder="" height={200} maxLetters={300} width={300}
                                                value={this.state.currentDescription} onChange={(event) => this.setState({ currentDescription: event.target.value })} zindex={1} />
                                    </div>
                                </div>
                                <div className="account-members-section fill">
                                    <MembersSection ref={this.membersSectionRef} tokens={this.props.tokens} inputWidth={250} />
                                </div>
                            </div>
                            <TagSection ref={this.tagSectionRef} tokens={this.props.tokens} onError={this.props.onError} zIndex={2} tagType="Projects" />
                            <ImageSection ref={this.imageSectionRef} imageSize={40} />
                            <LinkSection ref={this.linkSectionRef} />
                            <div className="center-horizontal">
                                <Button text="Create" onClick={this.handleSubmitEdit} />
                            </div>
                        </Dialog>
                    </div>    
                }
                {
                    this.state.showReportDialog &&
                    <ReportDialog onClose={() => this.setState({ showReportDialog: false })} onNotifcation={this.props.onNotifcation} />
                }
                {
                    this.state.showFollowersDialog &&
                    <UsersDialog title="Followers" userIds={this.state.user.followerIds} tokens={this.props.tokens}
                        onClose={() => this.setState({ showFollowersDialog: false })} onError={this.props.onError} onElementClicked={this.props.onJumpToAccount} />
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

function withParams(Component) {
    return props => <Component {...props} params={useParams()} />;
}

export default withParams(Account);