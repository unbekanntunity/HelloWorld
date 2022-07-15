import React, { Component } from 'react';

import heart from '../images/heart.png';
import report from '../images/error.png';
import reply from '../images/reply.png';
import remove from '../images/delete.png';
import filledHeart from '../images/filled-heart.png';

import VisibilitySensor from 'react-visibility-sensor';

import './Comment.css';

class Comment extends Component {
    state = {
        visibility: true
    }

    render() {
        console.log()
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
                <div className="comment-container" style={{
                    opacity: this.state.visibility ? 1 : 0.25,
                    transition: 'opacity 500ms linear',
                    width: this.props.width
                }}>
                    <div className="flex fill">
                        <img src={this.props.creatorImage} alt="" height={30} width={30} />
                        <div className="comment-margin-left fill">
                            <div className="comment-creator-general">
                                <p className="comment-creator-name revert-margin">{this.props.creatorName}</p>
                                <p className="comment-margin-left bold-gray-date revert-margin fill">10 min ago</p>
                                <div className="comment-likes-container">
                                    <img className="margin-right" src={this.props.usersLikedIds?.findIndex(id => id === this.props.sessionUserId) !== -1 ? filledHeart : heart}
                                        alt="" height={15} width={15} onClick={() => this.props.onLike(this.props.keyProp)}/>
                                    <p className="comment-likes revert-margin">{this.props.usersLikedIds?.length ?? 0}</p>
                                </div>
                            </div>
                            <p className="comment-content">{this.props.content}</p>
                            <div className="comment-actions">
                                <img className="margin-right" src={reply} alt="" height={20} width={20} onClick={() => this.props.onReplyClick(this.props.id, this.props.creatorName)} />
                                <img className="margin-right" src={report} alt="" height={20} width={20} onClick={this.props.onReportClick} />
                                {
                                    this.props.ownComment &&
                                    <img src={remove} alt="" height={20} width={20} onClick={() => this.props.onRemoveClick(this.props.id)} />
                                }
                            </div>
                        </div>
                        
                    </div>
                    <div className="comment-replies">
                    {
                        this.props.replies.map((item, index) => 
                            <Comment keyProp={index} id={item.id} key={index} creatorName={item.creatorName} creatorImage={item.creatorImage}
                                content={item.content} replies={item.replies}
                                ownComment={this.props.userId === item.creatorId} onRemoveClick={() => this.props.onRemoveReplyClick(item.id, this.props.keyProp)}
                                onFirstAppear={() => this.props.onFirstAppearReply(this.props.keyProp, index)} onReportClick={this.props.onReportClick}
                                onReplyClick={() => this.props.onReplyClick(this.props.id, this.props.creatorName)} />
                        )
                        }
                    </div>
                </div>
            </VisibilitySensor>
        )
    }
}

export default Comment;