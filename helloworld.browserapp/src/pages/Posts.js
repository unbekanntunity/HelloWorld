import React, { Component, createRef } from 'react';

import menuOpened from '../images/close.png';
import menuClosed from '../images/dots-vertical.png';
import listView from '../images/list.png';
import gridView from '../images/grid.png';
import add from '../images/plus.png';
import startPreview from '../images/eye-open.png';
import stopPreview from '../images/eye-close.png';

import SpeedDial from '../components/SpeedDial';
import TagSection from '../components/TagSection';
import ImageSection from '../components/ImageSection';
import MultiInputField from '../components/MultiInputField';
import LeftBanner from '../components/LeftBanner';
import { Post } from '../components/Post';
import { Dialog, ReportDialog } from '../components/Dialog';

import { Button, RoundButton } from '../components/Button';
import { sendFORMRequest, sendJSONRequest  } from '../requestFuncs';

import './Posts.css';

class Posts extends Component {

    state = {
        viewMode: "list",
        showReportDialog: false,
        showCreatePostDialog: false,
        showStopPreviewButton: false,
        existsCreatePostDialog: true,
        maxLetters: 300,

        content: "",
        posts: []
    }

    constructor(props) {
        super(props);

        this.tagSectionRef = createRef();
        this.imageSectionRef = createRef();
    }

    componentDidMount() {
        this.getPosts();
    }
   

    getPosts = () => {
        sendJSONRequest("GET", "/post/get_all", undefined, this.props.tokens.token).then(response => {
            this.setState({ posts: response.data })
            console.log(response);
        }, error => {
            console.log(error);
            this.props.onError(error.message)
        });
    }

    handleMulitLineChange = (event) => {
        this.setState({ content: event.target.value })
    }

    handleCreatePost = () => {
        this.setState({
            showCreatePostDialog: true,
            existsCreatePostDialog: true
        })
    }

    handleOnViewChange = () => {
        this.setState({
            viewMode: this.state.viewMode === "list" ? "grid" : "list"
        })
    }

    handleSubmit = () => {
        if (this.state.content.lenght > this.state.maxLetters) {
            this.props.onError("Content is too long.")
            return;
        }

        let formData = new FormData();

        console.log(this.state.content);


        formData.append("title", "Post");
        formData.append("content", this.state.content);

        let tagNames = this.tagSectionRef.current.getTags();
        for (var i = 0; i < tagNames.length; i++) {
            formData.append("tagNames", tagNames[i]);
        }

        let rawImages = this.imageSectionRef.current.getImages();
        for (var i = 0; i < rawImages.length; i++) {
            formData.append("rawImages", rawImages[i]);
        }

        if (!this.state.content && !rawImages.length) {
            this.props.onError("You need to have some content")
            return;
        }

        sendFORMRequest("POST", "/post/create", formData, this.props.tokens.token)
            .then(
                response => {
                    console.log(response);

                    this.setState({
                        posts: [...this.state.posts, response.data],
                        showCreatePostDialog: false,
                        existsCreatePostDialog: false,
                    });
                    this.getPosts();
                },
                error => {
                    console.log(error);
                    this.props.onError(error.message);
                }
            );
    }

    handleCreatorInfos = (index) => {
        if (this.state.showStopPreviewButton) {
            return;
        }

        let newPosts = this.state.posts;

        sendJSONRequest("GET", `/users/get/${this.state.posts[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newPosts[index].creatorImage = response.data.image;
                newPosts[index].creatorName = response.data.userName;
                this.setState({ posts: newPosts })
            }, error => {
                this.props.onError(error.message);
            }); 
    }

    handleStartPreview = () => {
        const previewPost = {
            title: "Post",
            content: this.state.content,
            tags: [...this.tagSectionRef.current.getTags()].map((tagName) => { return { name: tagName } }),
            rawImages: [...this.imageSectionRef.current.getImages()],
            creatorName: this.props.user.userName,
            creatorImage: this.props.user.image
        }

        if (!this.validatePost()) {
            return
        }

        this.setState({
            savedPosts: this.state.posts,
            posts: [previewPost],
            showCreatePostDialog: false,
            showStopPreviewButton: true
        })
    }

    handleStopPreview = () => {
        this.setState({
            posts: this.state.savedPosts,
            savedPosts: [],
            showCreatePostDialog: true,
            showStopPreviewButton: false,
        })
    }

    handleReport = () => {
        this.setState({ showReportDialog: !this.state.showReportDialog });
    }

    handleShare = () => {
        this.setState({ showShareDialog: !this.state.showShareDialog });
    }

    handleMulitLineChange = (event) => {
        this.setState({ });
    }

    handleReasonSelected = (reason) => {
        this.setState({ showReportSentDialog: !this.state.showReportSentDialog });
    }

    validatePost = () => {
        const result = this.state.content.length !== 0 || [...this.imageSectionRef.current.getImages()].length !== 0;

        if (!result) {
            this.props.onError( "You can not let the content and the image section empty");
        }

        return result
    }

    render() {
        return (
            <div className="page-body flex">
                <div>
                    <LeftBanner text="Posts" />
                </div>
                <div className="center-wrapper">
                    <div className={this.state.viewMode === "list" ? "posts-list" : "posts-grid"}>
                        {
                            this.state.posts.map((item, index) =>
                                <div key={index} className="posts-post-container">
                                    <Post keyProp={index} creatorPic={item.creatorImage} creatorName={item.creatorName}
                                        tags={item.tags} images={item.imageUrls} imageHeight={200} imageWidth={280} text={item.content} width="100%"
                                        onFirstAppear={this.handleCreatorInfos} onReportClick={() => this.setState({ showReportDialog: true })} />
                                </div>
                            )
                        }
                    </div>
                </div>
                <div className="actionMenu">
                    {
                        !this.state.showStopPreviewButton &&
                        <SpeedDial radius={60} iconSize={30} itemFactor={.75}
                            menuOpenedIcon={menuOpened} menuClosedIcon={menuClosed} >
                                <SpeedDial.Item icon={add} onClick={this.handleCreatePost} />
                                <SpeedDial.Item icon={this.state.viewMode === "list" ? listView : gridView} onClick={this.handleOnViewChange} />
                        </SpeedDial>
                    }
                    {
                        this.state.showStopPreviewButton &&
                        <RoundButton icon={stopPreview} radius={60} iconSize={30} onClick={this.handleStopPreview} />
                    }
                </div>
                {
                    this.state.existsCreatePostDialog &&
                    <div className="dialog-container" style={{ display: this.state.showCreatePostDialog ? 'inherit' : 'none' }} >
                            <Dialog title="Create new post" height="fit-content" width="400px" paddingX="20px" paddingY="20px"
                                onBackClick={() => this.setState({
                                    showCreatePostDialog: false,
                                    existsCreatePostDialog: false,
                                })} rightIcon={startPreview} onRightClick={this.handleStartPreview} backButton={true }>
                                <TagSection ref={this.tagSectionRef} tokens={this.props.tokens} onError={this.props.onError} zIndex={2} tagType="Posts"/>
                                <div className="posts-multi-container">
                                    <p className="posts-multi-label">Content</p>
                                    <MultiInputField placeholder="" height="200px" maxLetters={this.state.maxLetters}
                                        onChange={(event) => this.setState({ content: event.target.value })} />
                                </div>
                                <div className="posts-imageSection">
                                    <ImageSection ref={this.imageSectionRef} imageSize={40} />
                                </div>
                                <div className="posts-button-container">
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
        );
    }
}

export default Posts;