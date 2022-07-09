import React, { Component } from 'react';

import heart from '../images/heart.png';
import menuClosed from '../images/dots-vertical.png';
import menuOpened from '../images/dots-horizontal.png';
import DropDown from './DropDown';
import report from '../images/error.png';
import save from '../images/bookmark.png';
import rightArrow from '../images/right-arrow2.png';
import share from '../images/share.png';

import VisibilitySensor from 'react-visibility-sensor';

import Tag from './Tag';

import './Discussion.css';
import Header from './Header';

class Discussion extends Component {
    state = {
        visibility: true
    }

    render() {
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
            <div className="discussion-container" style={{
                opacity: this.state.visibility ? 1 : 0.25,
                transition: 'opacity 500ms linear',
                width: this.props.width
                }}>
                <Header creatorImage={this.props.creatorImage} title={this.props.title} tags={this.props.tags}
                    onReportClick={this.props.onReportClick} onRightArrowClick={this.props.onRightArrowClick} saveClick={this.props.saveClick}
                    onShareClick={this.props.onShareClick} />
               
                <div className="description-container">
                    <p>{this.props.startMessage}</p>
                    <p className="discussion-description-date">{this.props.createdAt}</p>
                </div>
                {
                    this.props.lastMessage &&
                    <div className="discussion-lastMessage-container">
                        <div className="discussion-lastMessage">
                            <b>{this.props.lastMessageAuthor}</b>
                            <p>: {this.props.lastMessage}</p>
                        </div>
                        <p className="discussion-lastMessage-date">{this.props.lastMessageCreated}</p>
                    </div>
                }
                </div>
            </VisibilitySensor>
        )
    }
}

export default Discussion;