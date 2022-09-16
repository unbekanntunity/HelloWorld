import React, { Component } from 'react';
import { handleUpdateRating, sendJSONRequest } from '../requestFuncs';

import Tag from './Tag';
import ImageSlider from './ImageSlider';
import Comment from './Comment';
import InputField from './InputField/InputField';

import heart from '../images/heart.png';
import menuClosed from '../images/dots-vertical.png';
import menuOpened from '../images/dots-horizontal.png';
import DropDown from './DropDown';
import report from '../images/error.png';
import unfollow from '../images/minus.png';
import rightArrow from '../images/right-arrow2.png';
import share from '../images/share.png';
import emoji from '../images/emoji.png';
import send from '../images/send.png';
import filledHeart from '../images/filled-heart.png';
import remove from '../images/delete.png';
import save from '../images/bookmark.png';
import saved from '../images/bookmarked.png';

import VisibilitySensor from 'react-visibility-sensor';

import './Post.css';
import { formatDate } from '../util';

export class Post extends Component {
    state = {
        visibility: true,
        menuOpen: false,
    }

    handleMenu = () => {
        this.setState({
            menuOpen: !this.state.menuOpen,
        });
    }

    render() {
        let { id, content, tags, creatorImage, creatorName, createdAt, creatorId, imageUrls, usersLikedIds } = this.props.item;
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
                <div className="post-container" style={{
                    opacity: this.state.visibility ? 1 : 0.25,
                    transition: 'opacity 500ms linear',
                    width: this.props.width
                }}>
                    <div className="post-header">
                        <img className="post-owner-pic" src={creatorImage} alt="" width={40} height={40} />
                        <div className="post-owner">
                            <h6 className="post-owner-name">{creatorName}</h6>
                            <p className="post-posted-at">{formatDate(createdAt)}</p>
                        </div>
                        <div className="post-header-menu">
                            {
                                tags && tags.length > 0 && <Tag margin="0px 10px" name={tags[0]} />
                            }
                            {
                                this.props.previewMode &&
                                <img src={heart} height={30} width={30} alt="" />
                            }
                            {
                                !this.props.previewMode &&
                                <img src={usersLikedIds?.findIndex(id => id === this.props.sessionUserId) !== -1 ? filledHeart : heart} width={30} height={30} alt=""
                                    onClick={() => this.props.onLike(this.props.keyProp)} />
                            }
                            <p className="header-likes">{usersLikedIds?.length ?? 0}</p>
                            {
                                this.props.previewMode &&
                                <img src={menuClosed} height={30} width={30} alt="" />
                            }
                            {
                                !this.props.previewMode &&
                                <DropDown toggleButton={{
                                    icon: undefined,
                                    arrowIconOpen: menuOpened,
                                    arrowIconClose: menuClosed
                                    }}
                                    arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                    {
                                        this.props.sessionUserId === creatorId &&
                                        <DropDown.Item icon={remove} textColor="red" text="Delete" iconSize={30} onClick={() => this.props.onDelete(this.props.keyProp)} />
                                    }
                                    <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                    <DropDown.Item icon={unfollow} textColor="red" text="Unfollow" iconSize={30} onClick={this.props.onUnfollowClick} />
                                    <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                                    <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                                    <DropDown.Item icon={this.props.saved ? saved : save} text="Save" iconSize={30} onClick={() => this.props.onSave(id)} />
                                        
                                </DropDown>
                            }
                        </div>
                    </div>

                    <p className="post-text">{content}</p>
                    {
                        imageUrls &&
                        <ImageSlider images={imageUrls} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                    }
                </div>
            </VisibilitySensor>
        );
    }
}

export class DetailedPost extends Component {
    state = {
        visibility: true,
        menuOpen: false,
        savedIcon: save,

        comments: [],

        currentComment: "",
        currentReply: "",
    }

    handleMenu = () => {
        this.setState({
            menuOpen: !this.state.menuOpen,
        });
    }

    componentDidMount() {
        this.getComments();
    }
    
    getComments = () => {
        sendJSONRequest("GET", '/comment/get_all/', undefined, this.props.tokens.token, {
            postId: this.props?.item.id
        }).then(response => {
            this.setState({ comments: response.data });
        }, error => {
            this.props.onError(error.message);
        })
    }

    handleCreatorInfos = (index) => {
        let newComments = this.state.comments;
        sendJSONRequest("GET", `/user/get/${newComments[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newComments[index].creatorImage = response.data.imageUrl;
                newComments[index].creatorName = response.data.userName;
                this.setState({
                    comments: newComments,
                })
            }, error => {
                this.props.onError(error.message);
            });
    }

    handleCreatorInfosForReply = (commentIndex, index) => {
        let newComments = this.state.comments;
        
        sendJSONRequest("GET", `/user/get/${newComments[commentIndex].replies[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newComments[commentIndex].replies[index].creatorImage = response.data.imageUrl;
                newComments[commentIndex].replies[index].creatorName = response.data.userName;
                this.setState({
                    comments: newComments,
                })
            }, error => {
                this.props.onError(error.message);
            });
    }

    handleSendComment = () => {
        if (!this.state.currentComment.length) {
            return;
        }

        if (this.state.currentReply === "") {
            sendJSONRequest("POST", `/comment/create/${this.props.item.id}`, {
                content: this.state.currentComment
            }, this.props.tokens.token).then(response => {
                console.log(response.data);

                this.setState({
                    comments: [...this.state.comments, response.data],
                    currentComment: ""
                });
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            })
        }
        else {
            sendJSONRequest("POST", `/reply/create/comment/${this.state.currentReply}`, {
                content: this.state.currentComment
            }, this.props.tokens.token)
                .then(response => {
                    let newComments = this.state.comments;
                    let comment = newComments.find(item => item.id === response.data.repliedOnId);
                    comment.replies = [...comment.replies, response.data]

                    this.setState({
                        currentReply: "",
                        currentComment: ""
                    })
                }, error => {
                    console.log(error);
                    this.props.onError(error.message);
                })
        }
    }

    handleSuccessRating = (index, response) => {
        let newComments = this.state.comments;
        newComments[index].usersLikedIds = response.data.usersLikedIds;
        this.setState({ comments: newComments })
    }

    handleRemoveComment = (id) => {
        sendJSONRequest("DELETE", `/comment/delete/${id}`, undefined, this.props.tokens.token)
            .then(_ => {
                this.setState({
                    comments: this.state.comments.filter(item => item.id !== id)
                })
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            })
    }

    handleRemoveReply = (id, commentIndex) => {
        sendJSONRequest("DELETE", `/reply/delete/${id}`, undefined, this.props.tokens.token)
            .then(_ => {
                let newComments = this.state.comments;
                newComments[commentIndex].replies = newComments[commentIndex].replies.filter(item => item.id !== id);
                this.setState({
                    comments: newComments
                })
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            })
    }

    handleReply = (id, creatorName) => {
        this.setState({
            currentComment: `@${creatorName} `,
            currentReply: id
        })
    }

    render() {
        let { id, content, tags, creatorImage, createdAt, creatorId, imageUrls, usersLikedIds } = this.props.item;

        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
                <div className="detailed-post-container" style={{
                    opacity: this.state.visibility ? 1 : 0.25,
                    transition: 'opacity 500ms linear',
                    width: this.props.width
                }}>
                    <div className="detailed-post-post-section">
                        <div className="post-header">
                            <img className="post-owner-pic" src={creatorImage} alt="" width={40} height={40} />
                            <div className="post-owner">
                                <div className="detailed-post-tags">
                                {
                                    tags?.map((item, index) =>
                                        <Tag key={index} margin="0px 10px 0px 0px" paddingY="2px" name={item} />)
                                    }
                                </div>
                                <p className="detailed-post-posted-at">{formatDate(createdAt)}</p>
                            </div> 
                            <div className="post-header-menu">
                                <div className="detailed-post-header-saved">
                                    {
                                        this.props.previewMode &&
                                        <img src={save} height={30} width={30} alt="" />
                                    }
                                    {
                                        !this.props.previewMode &&
                                        <img src={this.props.saved ? saved : save} alt="" height={30} width={30} onClick={() => this.props.onSave(id)} />
                                    }
                                </div>
                                <div className="flex">
                                    {
                                        this.props.previewMode &&
                                        <img src={heart} height={30} width={30} alt="" />
                                    }
                                    {
                                        !this.props.previewMode &&
                                        <img src={usersLikedIds?.findIndex(id => id === this.props.sessionUserId) !== -1 ? filledHeart : heart}
                                            width={30} height={30} alt="" onClick={() => this.props.onLike(this.props.keyProp)} />
                                    }
                                    <p className="header-likes" style={{ marginLeft: 5 }}>{usersLikedIds?.length ?? 0}</p>
                                </div>
                                {
                                    this.props.previewMode &&
                                    <img src={menuClosed} height={30} width={30} alt="" />
                                }
                                {
                                    !this.props.previewMode &&
                                    <DropDown toggleButton={{
                                        icon: undefined,
                                        arrowIconOpen: menuOpened,
                                        arrowIconClose: menuClosed
                                    }}
                                        arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                        {
                                            this.props.sessionUserId === creatorId &&
                                            <DropDown.Item icon={remove} textColor="red" text="Delete" iconSize={30} onClick={() => this.props.onDelete(this.props.keyProp)} />
                                        }
                                        <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                        <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                                    </DropDown>
                                }
                            </div>
                        </div>
                        <p className="post-text">{content}</p>
                        {
                            imageUrls &&
                            <ImageSlider images={imageUrls} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                        }
                    </div>
                    <div className="detailed-post-comments-section">
                        <div className="detailed-post-comments-container">
                            {
                                this.state.comments?.map((item, index) => {
                                    return (
                                        <div key={index} className="detailed-post-comment">
                                            <Comment keyProp={index} id={item.id} creatorName={item.creatorName} creatorImage={item.creatorImage}
                                                content={item.content} replies={item.replies} sessionUserId={this.props.sessionUserId} usersLikedIds={item.usersLikedIds}
                                                ownComment={this.props.sessionUserId === item.creatorId} ownReply={this.props.checkOwn}
                                                onFirstAppearReply={this.handleCreatorInfosForReply} onFirstAppear={this.handleCreatorInfos}
                                                onRemoveClick={this.handleRemoveComment} onRemoveReplyClick={this.handleRemoveReply}
                                                onReportClick={this.props.onReportClick} onReplyClick={this.handleReply}
                                                onLike={(index) => handleUpdateRating(item.id, "comment", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                                    </div>
                                    )
                                }
                                )
                            }
                        </div>
                        <div className="detailed-post-comment-input">
                            <img className="detailed-post-comment-emoji" src={emoji} alt="" height={25} width={25} />
                            <InputField value={this.state.currentComment} design="m2" width="100%" showUnderline={false} fill={true} placeholder="Leave a comment.."
                                onChange={(event) => this.setState({ currentComment: event.target.value })} />
                            {
                                this.props.previewMode && 
                                <img className="" src={send} alt="" height={25} width={25} />
                            }
                            {
                                !this.props.previewMode && 
                                <img className="" src={send} alt="" height={25} width={25} onClick={this.handleSendComment} />
                            }
                        </div>
                    </div>
                 </div>
            </VisibilitySensor>
        )
    }
}
