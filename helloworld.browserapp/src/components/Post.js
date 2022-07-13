import React, { Component, createRef } from 'react';
import { sendFORMRequest, sendJSONRequest } from '../requestFuncs';

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
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
                <div className="post-container" style={{
                    opacity: this.state.visibility ? 1 : 0.25,
                    transition: 'opacity 500ms linear',
                    width: this.props.width
                }}>
                    <div className="post-header">
                        <img className="post-owner-pic" src={this.props.creatorPic} alt="" width={40} height={40} />
                        <div className="post-owner">
                            <h6 className="post-owner-name">{this.props.creatorName}</h6>
                            <p className="post-posted-at">24.06</p>
                        </div>
                        <div className="post-header-menu">
                            {
                                this.props.tags && this.props.tags.length > 0 && <Tag name={this.props.tags[0].name} />
                            }
                            <img src={heart} width={30} height={30} alt="" />
                            <p className="header-likes">100</p>
                            <DropDown toggleButton={{
                                icon: undefined,
                                arrowIconOpen: menuOpened,
                                arrowIconClose: menuClosed
                            }}
                                arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                <DropDown.Item icon={unfollow} textColor="red" text="Unfollow" iconSize={30} onClick={this.props.onUnfollowClick} />
                                <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                                <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                            </DropDown>
                        </div>
                    </div>

                    <p className="post-text">{this.props.text}</p>
                    {
                        this.props.images &&
                        <ImageSlider images={this.props.images} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
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

        comments: [
        ],

        currentComment: "",
        currentReply: "",
    }

    constructor(props) {
        super(props);
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
            postId: this.props.id
        }).then(response => {
            this.setState({ comments: response.data });
        }, error => {
            this.props.onError(error.message);
        })
    }

    handleCreatorInfos = (index) => {
        let newComments = this.state.comments;
        sendJSONRequest("GET", `/users/get/${newComments[index].creatorId}`, undefined, this.props.tokens.token)
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
        
        sendJSONRequest("GET", `/users/get/${newComments[commentIndex].replies[index].creatorId}`, undefined, this.props.tokens.token)
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
            sendJSONRequest("POST", `/comment/create/${this.props.id}`, {
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
        console.log(creatorName)
        console.log(id)

        this.setState({
            currentComment: `@${creatorName} `,
            currentReply: id
        })
    }

    render() {
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
                <div className="detailed-post-container" style={{
                    opacity: this.state.visibility ? 1 : 0.25,
                    transition: 'opacity 500ms linear',
                    width: this.props.width
                }}>
                    <div className="detailed-post-post-section">
                        <div className="post-header">
                            <img className="post-owner-pic" src={this.props.creatorImage} alt="" width={40} height={40} />
                            <div className="post-owner">
                                <div className="detailed-post-tags">
                                {
                                    this.props.tags.map((item, index) =>
                                        <Tag key={index} paddingY="2px" name={item.name} />)
                                    }
                                </div>
                                <p className="detailed-post-posted-at">{formatDate(this.props.createdAt)}</p>
                            </div>
                            <div className="post-header-menu">
                                <img src={heart} width={30} height={30} alt="" />
                                <p className="header-likes" style={{ marginLeft: 5 }}>100</p>
                                <DropDown toggleButton={{
                                    icon: undefined,
                                    arrowIconOpen: menuOpened,
                                    arrowIconClose: menuClosed
                                }}
                                    arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                    <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                    <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                                </DropDown>
                            </div>
                        </div>
                        <p className="post-text">{this.props.text}</p>
                        {
                            this.props.images &&
                            <ImageSlider images={this.props.images} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                        }
                    </div>
                    <div className="detailed-post-comments-section">
                        <div className="detailed-post-comments-container">
                        {
                                this.state.comments.map((item, index) =>
                                    <div key={index} className="detailed-post-comment">
                                        <Comment keyProp={index} id={item.id} creatorName={item.creatorName} creatorImage={item.creatorImage}                                             content={item.content} replies={item.replies} userId={ this.props.userId}
                                            ownComment={this.props.userId === item.creatorId} ownReply={this.props.checkOwn}
                                            onFirstAppearReply={this.handleCreatorInfosForReply} onFirstAppear={this.handleCreatorInfos}
                                            onRemoveClick={this.handleRemoveComment} onRemoveReplyClick={this.handleRemoveReply}
                                            onReportClick={this.props.onReportClick} onReplyClick={this.handleReply} />
                                    </div>
                            )
                            }
                        </div>
                        <div className="detailed-post-comment-input">
                            <img className="detailed-post-comment-emoji" src={emoji} alt="" height={25} width={25} />
                            <InputField value={this.state.currentComment} design="m2" width="100%" showUnderline={false} fill={true} placeholder="Leave a comment.."
                                onChange={(event) => this.setState({ currentComment: event.target.value })} />
                            <img className="" src={send} alt="" height={25} width={25} onClick={this.handleSendComment} />
                        </div>
                    </div>
                 </div>
            </VisibilitySensor>
        )
    }
}
